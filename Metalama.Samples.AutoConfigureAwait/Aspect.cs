// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Caravela.Framework.Aspects;
using Caravela.Framework.Impl.CodeModel;
using Caravela.Framework.Impl.Sdk;
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