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
using Caravela.Framework.Sdk;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

[AttributeUsage(AttributeTargets.Assembly)]
public class AutoConfigureAwaitAttribute : Attribute, IAspect { }

[CompilerPlugin, AspectWeaver(typeof(AutoConfigureAwaitAttribute))]
class AutoConfigureAwaitWeaver : IAspectWeaver
{
    public CSharpCompilation Transform(AspectWeaverContext context) => new Rewriter(context.Compilation).VisitAllTrees(context.Compilation);

    class Rewriter : CSharpSyntaxRewriter
    {
        private readonly CSharpCompilation compilation;
        private readonly ITypeSymbol[] affectedTypes;

        public Rewriter(CSharpCompilation compilation)
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
    }
}