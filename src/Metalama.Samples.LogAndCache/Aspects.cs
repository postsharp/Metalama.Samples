// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Collections.Concurrent;
using System.Text;

public class LogAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        Console.WriteLine( meta.Target.Method.ToDisplayString() + " started." );

        try
        {
            var result = meta.Proceed();

            Console.WriteLine( meta.Target.Method.ToDisplayString() + " succeeded." );

            return result;
        }
        catch ( Exception e )
        {
            Console.WriteLine( meta.Target.Method.ToDisplayString() + " failed: " + e.Message );

            throw;
        }
    }
}

public class CacheAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        // Builds the caching string.
        var cacheKey = string.Format( GetCachingKeyFormattingString(), meta.Target.Parameters.Values.ToArray() );

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

    [CompileTimeOnly]
    private static string GetCachingKeyFormattingString()
    {
        var stringBuilder = meta.CompileTime( new StringBuilder() );
        stringBuilder.Append( meta.Target.Type.ToString() );
        stringBuilder.Append( '.' );
        stringBuilder.Append( meta.Target.Method.Name );
        stringBuilder.Append( '(' );

        var i = meta.CompileTime( 0 );

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

// Placeholder implementation of a cache because the hosted try.metalama.net does not allow for MemoryCache.
public static class SampleCache
{
    public static readonly ConcurrentDictionary<string, object> Cache =
        new();
}