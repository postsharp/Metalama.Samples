using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Project;
using System.Diagnostics.CodeAnalysis;

public partial class CachingOptions : ProjectExtension
{
    private static DiagnosticDefinition<IType> _error = new( "CACHE01", Severity.Error,                     /*[VerifyCacheKeyMember:Start]*/
        "The type '{0}' cannot be a part of a cache key. Implement ICacheKey, use [CacheKeyMember] or register a cache key builder." );

    internal bool VerifyCacheKeyMember<T>( T expression, IDiagnosticSink diagnosticSink )                     
        where T : IExpression, IDeclaration
    {
        if ( this._toStringTypes.Contains( expression.Type )  )
        {
            return true;
        }
        else if (  this._externalCacheBuilderTypes.ContainsKey( expression.Type ) )
        {
            return true;
        }
        else if ( expression.Type.Is( typeof( ICacheKey ) ) ||
                (expression.Type is INamedType namedType && namedType.Enhancements().HasAspect<GenerateCacheKeyAspect>()) )
        {
            return true;
        }
        else
        {
            diagnosticSink.Report( _error.WithArguments( expression.Type ), expression );
            return false;
        }                                                                                                       /*[VerifyCacheKeyMember:End]*/
    }

    internal bool TryGetCacheKeyExpression( IExpression expression, IExpression cacheKeyBuilderProvider,        /*[TryGetCacheKeyExpression:Start]*/
        [NotNullWhen(true)] out IExpression? cacheKeyExpression )
    {
        var expressionBuilder = new ExpressionBuilder();
        
        if ( this._toStringTypes.Contains( expression.Type ) )
        {
            expressionBuilder.AppendExpression( expression );

            if ( expression.Type.IsNullable == true )
            {
                expressionBuilder.AppendVerbatim( "?.ToString() ?? \"null\"" );
            }
        }
        else if ( this._externalCacheBuilderTypes.TryGetValue( expression.Type, out var externalCacheBuilderType ) )
        {
            expressionBuilder.AppendExpression( cacheKeyBuilderProvider );
            expressionBuilder.AppendVerbatim( ".GetCacheKeyBuilder<" );
            expressionBuilder.AppendTypeName( expression.Type );
            expressionBuilder.AppendVerbatim( ", " );
            expressionBuilder.AppendTypeName( externalCacheBuilderType );
            expressionBuilder.AppendVerbatim( ">(" );
            expressionBuilder.AppendExpression( expression );
            expressionBuilder.AppendVerbatim( ")" );

            if ( expression.Type.IsNullable == true )
            {
                expressionBuilder.AppendVerbatim( "?? \"null\"" );
            }
        }
        else if ( expression.Type.Is( typeof( ICacheKey ) ) ||
                (expression.Type is INamedType namedType && namedType.Enhancements().HasAspect<GenerateCacheKeyAspect>()) )
        {
            expressionBuilder.AppendExpression( expression );
            expressionBuilder.AppendVerbatim( ".ToCacheKey(" );
            expressionBuilder.AppendExpression( cacheKeyBuilderProvider );
            expressionBuilder.AppendVerbatim( ")" );

            if ( expression.Type.IsNullable == true )
            {
                expressionBuilder.AppendVerbatim( "?? \"null\"" );
            }
        }
        else
        {
            cacheKeyExpression = null;
            return false;
        }

    

        cacheKeyExpression = expressionBuilder.ToExpression();
            return true;
    }                                                               /*[TryGetCacheKeyExpression:End]*/
}
