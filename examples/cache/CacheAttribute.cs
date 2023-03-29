using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

public class CacheAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        // Builds the caching string.
        var cacheKey = GetCachingKeyFormattingString().ToValue();

        // Cache lookup.
        if ( SampleCache.Cache.TryGetValue( cacheKey, out object value ) )
        {
            // Cache hit.
            return value;
        }
        else
        {
            // Cache miss. Go and invoke the method.
            var result = meta.Proceed();

            // Add to cache.
            SampleCache.Cache.TryAdd( cacheKey, result );

            return result;
        }
    }

    private static InterpolatedStringBuilder GetCachingKeyFormattingString()
    {
        var stringBuilder = new InterpolatedStringBuilder();
        stringBuilder.AddText( meta.Target.Type.ToString() );
        stringBuilder.AddText( "." );
        stringBuilder.AddText( meta.Target.Method.Name );
        stringBuilder.AddText( "(" );

        var i = meta.CompileTime( 0 );

        foreach ( var p in meta.Target.Parameters )
        {
            var comma = i > 0 ? ", " : "";

            if ( p.RefKind == RefKind.Out )
            {
                stringBuilder.AddText( $"{comma}{p.Name} = <out> " );
            }
            else
            {
                stringBuilder.AddText( $"{comma}{{" );
                stringBuilder.AddExpression( p.Value );
                stringBuilder.AddText( "}" );
            }

            i++;
        }

        stringBuilder.AddText( ")" );

        return stringBuilder;
    }
}
