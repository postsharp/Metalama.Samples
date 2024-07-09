using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Text;

public class CacheAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        // Builds the caching string.
        var cacheKey = string.Format( GetCachingKeyFormattingString(),
            meta.Target.Parameters.ToValueArray() );

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

    [CompileTime]
    private static string GetCachingKeyFormattingString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append( meta.Target.Type );
        stringBuilder.Append( '.' );
        stringBuilder.Append( meta.Target.Method.Name );
        stringBuilder.Append( '(' );

        var i = 0;

        foreach ( var p in meta.Target.Parameters )
        {
            var comma = i > 0 ? ", " : "";

            if ( p.RefKind == RefKind.Out )
            {
                stringBuilder.Append( $"{comma}{p.Name} = <out> " );
            }
            else
            {
                stringBuilder.Append( $"{comma}{{{i}}}" );
            }

            i++;
        }

        stringBuilder.Append( ')' );

        return stringBuilder.ToString();
    }
}