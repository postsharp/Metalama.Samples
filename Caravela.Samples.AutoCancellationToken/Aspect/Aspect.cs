using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Caravela.Framework.Eligibility;
using Caravela.Framework.Impl.Sdk;
using Caravela.Framework.Impl.CodeModel;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class AutoCancellationTokenAttribute : Attribute, IAspect<INamedType>
{
    public void BuildAspect(IAspectBuilder<INamedType> builder)
    {
    }

    public void BuildAspectClass(IAspectClassBuilder builder)
    {
    }

    public void BuildEligibility(IEligibilityBuilder<INamedType> builder)
    {
    }
}

[CompilerPlugin, AspectWeaver(typeof(AutoCancellationTokenAttribute))]
class AutoCancellationTokenWeaver : IAspectWeaver
{
    public void Transform(AspectWeaverContext context)
    {
        var compilation = context.Compilation;
        var instancesNodes = context.AspectInstances.SelectMany(a => a.TargetDeclaration.GetSymbol().DeclaringSyntaxReferences).Select(r=>r.GetSyntax()).Cast<CSharpSyntaxNode>();
        RunRewriter(new AnnotateNodesRewriter(instancesNodes));
        RunRewriter(new AddCancellationTokenToMethodsRewriter(compilation.Compilation));
        RunRewriter(new AddCancellationTokenToInvocationsRewriter(compilation.Compilation));
        context.Compilation = compilation;

        void RunRewriter(RewriterBase rewriter) => compilation = rewriter.Visit(compilation);
    }

    abstract class RewriterBase : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) => this.VisitTypeDeclaration(node, base.VisitInterfaceDeclaration);
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) => this.VisitTypeDeclaration(node, base.VisitClassDeclaration);
        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node) => this.VisitTypeDeclaration(node, base.VisitStructDeclaration);
        public override SyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax node) => this.VisitTypeDeclaration(node, base.VisitRecordDeclaration);

        protected abstract T VisitTypeDeclaration<T>(T node, Func<T, SyntaxNode> baseVisit) where T : TypeDeclarationSyntax;

        protected static readonly TypeSyntax CancellationTokenType = ParseTypeName(typeof(CancellationToken).FullName);

        protected static bool IsCancellationToken(IParameterSymbol parameter) => parameter.OriginalDefinition.Type.ToString() == typeof(CancellationToken).FullName;

        // Make sure VisitInvocationExpression is not called for expressions inside members that are not methods
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) => node;
        public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node) => node;
        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node) => node;
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) => node;
        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => node;
        public override SyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node) => node;

        protected const string CancellationAttributeName = "Caravela.Open.AutoCancellationToken.AutoCancellationTokenAttribute";

        public IPartialCompilation Visit(IPartialCompilation compilation)
            => compilation.UpdateSyntaxTrees((root, cancellationToken) => this.Visit(root));
    }

    sealed class AnnotateNodesRewriter : RewriterBase
    {
        private readonly HashSet<CSharpSyntaxNode> _instancesNodes;

        public AnnotateNodesRewriter(IEnumerable<CSharpSyntaxNode> instancesNodes)
        {
            this._instancesNodes = new(instancesNodes);
        }

        public static SyntaxAnnotation Annotation { get; } = new SyntaxAnnotation();

        protected override T VisitTypeDeclaration<T>(T node, Func<T, SyntaxNode> baseVisit)
        {
            if (!this._instancesNodes.Contains(node))
            {
                return node;
            }

            return node.WithAdditionalAnnotations(Annotation);
        }
    }

    sealed class AddCancellationTokenToMethodsRewriter : RewriterBase
    {
        private readonly Compilation _compilation;

        public AddCancellationTokenToMethodsRewriter(Compilation compilation)
        {
            this._compilation = compilation;
        }

        protected override T VisitTypeDeclaration<T>(T node, Func<T, SyntaxNode> baseVisit)
        {
            if (!node.HasAnnotation(AnnotateNodesRewriter.Annotation))
            {
                return node;
            }

            return (T)baseVisit(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var semanticModel = this._compilation.GetSemanticModel(node.SyntaxTree);

            var methodSymbol = semanticModel.GetDeclaredSymbol(node);

            if (!methodSymbol.IsAsync || methodSymbol.Parameters.Any(IsCancellationToken))
            {
                return node;
            }

            return node.AddParameterListParameters(Parameter(
                default, default, CancellationTokenType, Identifier("cancellationToken"),
                EqualsValueClause(LiteralExpression(SyntaxKind.DefaultLiteralExpression))));
        }
    }

    sealed class AddCancellationTokenToInvocationsRewriter : RewriterBase
    {
        private readonly Compilation _compilation;
        private string _cancellationTokenParameterName;

        public AddCancellationTokenToInvocationsRewriter(Compilation compilation)
        {
            this._compilation = compilation;
        }

        protected override T VisitTypeDeclaration<T>(T node, Func<T, SyntaxNode> baseVisit)
        {
            if (!node.HasAnnotation(AnnotateNodesRewriter.Annotation))
            {
                return node;
            }

            var semanticModel = this._compilation.GetSemanticModel(node.SyntaxTree);

            var symbol = semanticModel.GetDeclaredSymbol(node);

            var attributeData = symbol.GetAttributes().SingleOrDefault(a => a.AttributeClass.ToString() == CancellationAttributeName);

            return (T)baseVisit(node);
        }


        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var semanticModel = this._compilation.GetSemanticModel(node.SyntaxTree);

            var methodSymbol = semanticModel.GetDeclaredSymbol(node);

            if (!methodSymbol.IsAsync)
            {
                return node;
            }

            var cancellationTokenParmeters = methodSymbol.Parameters.Where(IsCancellationToken);

            if (cancellationTokenParmeters.Count() != 1)
            {
                return node;
            }

            this._cancellationTokenParameterName = cancellationTokenParmeters.Single().Name;

            return base.VisitMethodDeclaration(node);
        }

        public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) =>
            this.VisitFunction(node, false, base.VisitAnonymousMethodExpression);
        public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) =>
            this.VisitFunction(node, node.Modifiers.Any(SyntaxKind.StaticKeyword), base.VisitParenthesizedLambdaExpression);
        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) =>
            this.VisitFunction(node, node.Modifiers.Any(SyntaxKind.StaticKeyword), base.VisitSimpleLambdaExpression);
        public override SyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) =>
            this.VisitFunction(node, node.Modifiers.Any(SyntaxKind.StaticKeyword), base.VisitLocalFunctionStatement);

        private T VisitFunction<T>(T node, bool isStatic, Func<T, SyntaxNode> baseVisit) where T : SyntaxNode
        {
            if (isStatic)
            {
                return node;
            }

            return (T)baseVisit(node);
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var addCt = false;

            var semanticModel = this._compilation.GetSemanticModel(node.SyntaxTree);

            var invocationWithCt = node.AddArgumentListArguments(Argument(DefaultExpression(CancellationTokenType)));
            var newInvocationArgumentsCount = invocationWithCt.ArgumentList.Arguments.Count;

            var speculativeSymbol = semanticModel.GetSpeculativeSymbolInfo(node.SpanStart, invocationWithCt, default).Symbol as IMethodSymbol;

            if (
                // the code compiles
                speculativeSymbol != null &&
                // the added parameter corresponds to its own argument
                speculativeSymbol.Parameters.Length >= newInvocationArgumentsCount &&
                // that argument is CancellationToken
                IsCancellationToken(speculativeSymbol.Parameters[newInvocationArgumentsCount - 1]))
            {
                addCt = true;
            }

            node = (InvocationExpressionSyntax)base.VisitInvocationExpression(node);

            if (addCt)
            {
                node = node.AddArgumentListArguments(Argument(IdentifierName(this._cancellationTokenParameterName)));
            }

            return node;
        }
    }
}