// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Framework.Engine.Sdk;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface )]
public class AutoCancellationTokenAttribute : TypeAspect { }

[CompilerPlugin]
[AspectWeaver( typeof(AutoCancellationTokenAttribute) )]
internal class AutoCancellationTokenWeaver : IAspectWeaver
{
    public void Transform( AspectWeaverContext context )
    {
        var compilation = context.Compilation;

        var roslynCompilation = compilation.Compilation;

        var instancesNodes = context.AspectInstances.Values
            .SelectMany( a => a.TargetDeclaration.GetSymbol( roslynCompilation )!.DeclaringSyntaxReferences )
            .Select( r => r.GetSyntax() )
            .Cast<CSharpSyntaxNode>();

        // Execute the chain of rewriter. Note that it is not a performance best practice to
        // create several compilations. It is more efficient to merge all rewrites into a single one.
        var compilation1 = new AnnotateNodesRewriter( instancesNodes ).Visit( compilation );
        var compilation2 = new AddCancellationTokenToMethodsRewriter( compilation1.Compilation ).Visit( compilation1 );
        var compilation3 = new AddCancellationTokenToInvocationsRewriter( compilation2.Compilation ).Visit( compilation2 );
        context.Compilation = compilation3;
    }

    private abstract class RewriterBase : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInterfaceDeclaration( InterfaceDeclarationSyntax node )
            => this.VisitTypeDeclaration( node, base.VisitInterfaceDeclaration );

        public override SyntaxNode? VisitClassDeclaration( ClassDeclarationSyntax node ) => this.VisitTypeDeclaration( node, base.VisitClassDeclaration );

        public override SyntaxNode? VisitStructDeclaration( StructDeclarationSyntax node ) => this.VisitTypeDeclaration( node, base.VisitStructDeclaration );

        public override SyntaxNode? VisitRecordDeclaration( RecordDeclarationSyntax node ) => this.VisitTypeDeclaration( node, base.VisitRecordDeclaration );

        protected abstract T? VisitTypeDeclaration<T>( T node, Func<T, SyntaxNode?> baseVisit )
            where T : TypeDeclarationSyntax;

        protected static readonly TypeSyntax CancellationTokenType = ParseTypeName( typeof(CancellationToken).FullName );

        protected static bool IsCancellationToken( IParameterSymbol parameter )
            => parameter.OriginalDefinition.Type.ToString() == typeof(CancellationToken).FullName;

        // Make sure VisitInvocationExpression is not called for expressions inside members that are not methods
        public override SyntaxNode VisitPropertyDeclaration( PropertyDeclarationSyntax node ) => node;

        public override SyntaxNode VisitIndexerDeclaration( IndexerDeclarationSyntax node ) => node;

        public override SyntaxNode VisitEventDeclaration( EventDeclarationSyntax node ) => node;

        public override SyntaxNode VisitFieldDeclaration( FieldDeclarationSyntax node ) => node;

        public override SyntaxNode VisitConstructorDeclaration( ConstructorDeclarationSyntax node ) => node;

        public override SyntaxNode VisitDestructorDeclaration( DestructorDeclarationSyntax node ) => node;

        protected string CancellationAttributeName { get; } = typeof(AutoCancellationTokenAttribute).FullName;

        public IPartialCompilation Visit( IPartialCompilation compilation ) => compilation.UpdateSyntaxTrees( ( root, _ ) => this.Visit( root ) );
    }

    private sealed class AnnotateNodesRewriter : RewriterBase
    {
        private readonly HashSet<CSharpSyntaxNode> _instancesNodes;

        public AnnotateNodesRewriter( IEnumerable<CSharpSyntaxNode> instancesNodes )
        {
            this._instancesNodes = new HashSet<CSharpSyntaxNode>( instancesNodes );
        }

        public static SyntaxAnnotation Annotation { get; } = new();

        protected override T VisitTypeDeclaration<T>( T node, Func<T, SyntaxNode?> baseVisit )
        {
            if ( !this._instancesNodes.Contains( node ) )
            {
                return node;
            }

            return node.WithAdditionalAnnotations( Annotation );
        }
    }

    private sealed class AddCancellationTokenToMethodsRewriter : RewriterBase
    {
        private readonly Compilation _compilation;

        public AddCancellationTokenToMethodsRewriter( Compilation compilation )
        {
            this._compilation = compilation;
        }

        protected override T? VisitTypeDeclaration<T>( T node, Func<T, SyntaxNode?> baseVisit )
            where T : class
        {
            if ( !node.HasAnnotation( AnnotateNodesRewriter.Annotation ) )
            {
                return node;
            }

            return (T?) baseVisit( node );
        }

        public override SyntaxNode VisitMethodDeclaration( MethodDeclarationSyntax node )
        {
            var semanticModel = this._compilation.GetSemanticModel( node.SyntaxTree );

            var methodSymbol = semanticModel.GetDeclaredSymbol( node )!;

            if ( !methodSymbol.IsAsync || methodSymbol.Parameters.Any( IsCancellationToken ) )
            {
                return node;
            }

            return node.AddParameterListParameters(
                Parameter(
                    default,
                    default,
                    CancellationTokenType,
                    Identifier( "cancellationToken" ),
                    EqualsValueClause( LiteralExpression( SyntaxKind.DefaultLiteralExpression ) ) ) );
        }
    }

    private sealed class AddCancellationTokenToInvocationsRewriter : RewriterBase
    {
        private readonly Compilation _compilation;
        private string? _cancellationTokenParameterName;

        public AddCancellationTokenToInvocationsRewriter( Compilation compilation )
        {
            this._compilation = compilation;
        }

        protected override T? VisitTypeDeclaration<T>( T node, Func<T, SyntaxNode?> baseVisit )
            where T : class
        {
            if ( !node.HasAnnotation( AnnotateNodesRewriter.Annotation ) )
            {
                return node;
            }

            var semanticModel = this._compilation.GetSemanticModel( node.SyntaxTree );

            var symbol = semanticModel.GetDeclaredSymbol( node )!;

            var attributeData = symbol.GetAttributes()
                .SingleOrDefault( a => a.AttributeClass?.ToString() == this.CancellationAttributeName );

            return (T?) baseVisit( node );
        }

        public override SyntaxNode? VisitMethodDeclaration( MethodDeclarationSyntax node )
        {
            var semanticModel = this._compilation.GetSemanticModel( node.SyntaxTree );

            var methodSymbol = semanticModel.GetDeclaredSymbol( node )!;

            if ( !methodSymbol.IsAsync )
            {
                return node;
            }

            var cancellationTokenParameters = methodSymbol.Parameters.Where( IsCancellationToken ).ToList();

            if ( cancellationTokenParameters.Count() != 1 )
            {
                return node;
            }

            this._cancellationTokenParameterName = cancellationTokenParameters.Single().Name;

            return base.VisitMethodDeclaration( node );
        }

        public override SyntaxNode? VisitAnonymousMethodExpression( AnonymousMethodExpressionSyntax node )
            => this.VisitFunction( node, false, base.VisitAnonymousMethodExpression );

        public override SyntaxNode? VisitParenthesizedLambdaExpression( ParenthesizedLambdaExpressionSyntax node )
            => this.VisitFunction( node, node.Modifiers.Any( SyntaxKind.StaticKeyword ), base.VisitParenthesizedLambdaExpression );

        public override SyntaxNode? VisitSimpleLambdaExpression( SimpleLambdaExpressionSyntax node )
            => this.VisitFunction( node, node.Modifiers.Any( SyntaxKind.StaticKeyword ), base.VisitSimpleLambdaExpression );

        public override SyntaxNode? VisitLocalFunctionStatement( LocalFunctionStatementSyntax node )
            => this.VisitFunction( node, node.Modifiers.Any( SyntaxKind.StaticKeyword ), base.VisitLocalFunctionStatement );

        private T? VisitFunction<T>( T node, bool isStatic, Func<T, SyntaxNode?> baseVisit ) where T : SyntaxNode
        {
            if ( isStatic )
            {
                return node;
            }

            return (T?) baseVisit( node );
        }

        public override SyntaxNode VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var addCt = false;

            var semanticModel = this._compilation.GetSemanticModel( node.SyntaxTree );

            var invocationWithCt = node.AddArgumentListArguments( Argument( DefaultExpression( CancellationTokenType ) ) );
            var newInvocationArgumentsCount = invocationWithCt.ArgumentList.Arguments.Count;

            if (

                // the code compiles
                semanticModel.GetSpeculativeSymbolInfo( node.SpanStart, invocationWithCt, default ).Symbol is
                    IMethodSymbol speculativeSymbol &&

                // the added parameter corresponds to its own argument
                speculativeSymbol.Parameters.Length >= newInvocationArgumentsCount &&

                // that argument is CancellationToken
                IsCancellationToken( speculativeSymbol.Parameters[newInvocationArgumentsCount - 1] ) )
            {
                addCt = true;
            }

            node = (InvocationExpressionSyntax) base.VisitInvocationExpression( node )!;

            if ( addCt )
            {
                node = node.AddArgumentListArguments( Argument( IdentifierName( this._cancellationTokenParameterName! ) ) );
            }

            return node;
        }
    }
}