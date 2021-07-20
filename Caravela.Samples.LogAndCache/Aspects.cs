using System;
using System.Text;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;

public class LogAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        Console.WriteLine(meta.Method.ToDisplayString() + " started.");

        try
        {
            dynamic result = meta.Proceed();

            Console.WriteLine(meta.Method.ToDisplayString() + " succeeded.");
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(meta.Method.ToDisplayString() + " failed: " + e.Message);

            throw;
        }
    }
}

public class CacheAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        // Builds the caching string.
        var cacheKey = string.Format(GetCachingKeyFormattingString(), meta.Parameters.Values.ToArray());

        // Cache lookup.
        if (SampleCache.Cache.TryGetValue(cacheKey, out object value))
        {
            // Cache hit.
            return value;
        }
        else
        {
            // Cache miss. Go and invoke the method.
            dynamic result = meta.Proceed();

            // Add to cache.
            SampleCache.Cache.TryAdd(cacheKey, result);
            return result;
        }
    }

    private static string GetCachingKeyFormattingString()
    {
        var stringBuilder = meta.CompileTime(new StringBuilder());
        stringBuilder.Append(meta.Type.ToString());
        stringBuilder.Append('.');
        stringBuilder.Append(meta.Method.Name);
        stringBuilder.Append('(');

        var i = meta.CompileTime(0);
        foreach (var p in meta.Parameters)
        {
            var comma = i > 0 ? ", " : "";

            if (p.IsOut())
            {
                stringBuilder.Append($"{comma}{p.Name} = <out> ");
            }
            else
            {
                stringBuilder.Append($"{comma}{{{i}}}");
            }

            i++;
        }
        stringBuilder.Append(')');
        return stringBuilder.ToString();
    }
}

// Placeholder implementation of a cache because the hosted try.postsharp.net does not allow for MemoryCache.
public static class SampleCache
{
    public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, object> Cache =
        new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
}