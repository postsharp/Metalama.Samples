using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Caravela;
using Caravela.Framework.Aspects;
using Caravela.Framework.Impl.Sdk;
using Caravela.Framework.Impl.CodeModel;
using Caravela.Framework.Code;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Caravela.Framework.Eligibility;

[AttributeUsage(AttributeTargets.Assembly)]
public class AutoConfigureAwaitAttribute : Attribute, IAspect<ICompilation>
{
    public void BuildAspect(IAspectBuilder<ICompilation> builder)
    {
        
    }

    public void BuildAspectClass(IAspectClassBuilder builder)
    {
        
    }

    public void BuildEligibility(IEligibilityBuilder<ICompilation> builder)
    {
        
    }
}

[CompilerPlugin, AspectWeaver(typeof(AutoConfigureAwaitAttribute))]
class AutoConfigureAwaitWeaver : IAspectWeaver
{
    public void Transform(AspectWeaverContext context)
    {
        context.Compilation = new Rewriter(context.Compilation.Compilation).Visit(context.Compilation);
    }

    class Rewriter : CSharpSyntaxRewriter
    {
        private readonly Compilation compilation;
        private readonly ITypeSymbol[] affectedTypes;

        public Rewriter(Compilation compilation)
        {
            this.compilation = compilation;

            affectedTypes = new[] { typeof(Task), typeof(Task<>), typeof(ValueTask), typeof(ValueTask<>) }
                .Select(t => compilation.GetTypeByMetadataName(t.FullName)).ToArray();
        }

        public override SyntaxNode VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            var expressionType = compilation.GetSemanticModel(node.SyntaxTree).GetTypeInfo(node.Expression).ConvertedType.OriginalDefinition;

            var awaitExpression = (AwaitExpressionSyntax)base.VisitAwaitExpression(node);

            if (affectedTypes.Contains(expressionType))
            {
                awaitExpression = awaitExpression.WithExpression(
                    // expression.ConfigureAwait(false)
                    InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, awaitExpression.Expression, IdentifierName("ConfigureAwait")))
                    .AddArgumentListArguments(Argument(LiteralExpression(SyntaxKind.FalseLiteralExpression))));
            }

            return awaitExpression;
        }

        public IPartialCompilation Visit(IPartialCompilation compilation)
            =>  compilation.UpdateSyntaxTrees( ( node, cancellationToken ) => this.Visit(node));
    }
}