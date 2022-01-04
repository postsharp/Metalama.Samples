// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Framework.Engine.Sdk;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public class AutoConfigureAwaitAttribute : CompilationAspect { }

[CompilerPlugin]
[AspectWeaver( typeof(AutoConfigureAwaitAttribute) )]
internal class AutoConfigureAwaitWeaver : IAspectWeaver
{
    public void Transform( AspectWeaverContext context )
    {
        context.Compilation = new Rewriter( context.Compilation.Compilation ).Visit( context.Compilation );
    }

    private class Rewriter : CSharpSyntaxRewriter
    {
        private readonly Compilation _compilation;
        private readonly ITypeSymbol[] _affectedTypes;

        public Rewriter( Compilation compilation )
        {
            this._compilation = compilation;

            this._affectedTypes = new[] { typeof(Task), typeof(Task<>), typeof(ValueTask), typeof(ValueTask<>) }
                .Select( t => compilation.GetTypeByMetadataName( t.FullName )! )
                .ToArray();
        }

        public override SyntaxNode? VisitAwaitExpression( AwaitExpressionSyntax node )
        {
            var expressionType = this._compilation.GetSemanticModel( node.SyntaxTree )
                .GetTypeInfo( node.Expression )!
                .ConvertedType!.OriginalDefinition;

            var awaitExpression = (AwaitExpressionSyntax) base.VisitAwaitExpression( node )!;

            if ( this._affectedTypes.Contains( expressionType, SymbolEqualityComparer.Default ) )
            {
                awaitExpression = awaitExpression.WithExpression(

                    // expression.ConfigureAwait(false)
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                awaitExpression.Expression,
                                IdentifierName( "ConfigureAwait" ) ) )
                        .AddArgumentListArguments( Argument( LiteralExpression( SyntaxKind.FalseLiteralExpression ) ) ) );
            }

            return awaitExpression;
        }

        public IPartialCompilation Visit( IPartialCompilation compilation ) => compilation.UpdateSyntaxTrees( ( node, _ ) => this.Visit( node ) );
    }
}