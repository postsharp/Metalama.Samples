using System;
using System.Linq;
using System.Threading.Tasks;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;
using Caravela.Framework.Eligibility;
using Caravela.Framework.Impl.CodeModel;
using Caravela.Framework.Impl.Sdk;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

[CompilerPlugin]
[AspectWeaver(typeof(AutoConfigureAwaitAttribute))]
internal class AutoConfigureAwaitWeaver : IAspectWeaver
{
    public void Transform(AspectWeaverContext context)
    {
        context.Compilation = new Rewriter(context.Compilation.Compilation).Visit(context.Compilation);
    }

    private class Rewriter : CSharpSyntaxRewriter
    {
        private readonly Compilation _compilation;
        private readonly ITypeSymbol[] _affectedTypes;

        public Rewriter(Compilation compilation)
        {
            this._compilation = compilation;

            this._affectedTypes = new[] { typeof(Task), typeof(Task<>), typeof(ValueTask), typeof(ValueTask<>) }
                .Select(t => compilation.GetTypeByMetadataName(t.FullName)!).ToArray();
        }

        public override SyntaxNode? VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            var expressionType = this._compilation.GetSemanticModel(node.SyntaxTree)
                .GetTypeInfo(node.Expression)!
                .ConvertedType!.OriginalDefinition;

            var awaitExpression = (AwaitExpressionSyntax)base.VisitAwaitExpression(node)!;

            if (this._affectedTypes.Contains(expressionType, SymbolEqualityComparer.Default))
            {
                awaitExpression = awaitExpression.WithExpression(
                    // expression.ConfigureAwait(false)
                    InvocationExpression(
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, awaitExpression.Expression,
                                IdentifierName("ConfigureAwait")))
                        .AddArgumentListArguments(Argument(LiteralExpression(SyntaxKind.FalseLiteralExpression))));
            }

            return awaitExpression;
        }

        public IPartialCompilation Visit(IPartialCompilation compilation)
            => compilation.UpdateSyntaxTrees((node, _) => this.Visit(node));
    }
}