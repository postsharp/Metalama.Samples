using System;
using System.Text;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;
using Caravela.Framework.Code.Syntax;

public class CacheAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        // Builds the caching string.
        var cacheKey = GetCachingKeyFormattingString().ToInterpolatedString();

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

    private static InterpolatedStringBuilder GetCachingKeyFormattingString()
    {
        var stringBuilder = InterpolatedStringBuilder.Create();
        stringBuilder.AddText(meta.Target.Type.ToString());
        stringBuilder.AddText(".");
        stringBuilder.AddText(meta.Target.Method.Name);
        stringBuilder.AddText("(");

        var i = meta.CompileTime(0);
        foreach (var p in meta.Target.Parameters)
        {
            var comma = i > 0 ? ", " : "";

            if (p.IsOut())
            {
                stringBuilder.AddText($"{comma}{p.Name} = <out> ");
            }
            else
            {
                stringBuilder.AddText($"{comma}{{");
                stringBuilder.AddExpression(p.Value);
                stringBuilder.AddText("}");
            }

            i++;
        }
        stringBuilder.AddText(")");
        return stringBuilder;
    }

 }

// Placeholder implementation of a cache because the hosted try.postsharp.net does not allow for MemoryCache.
static class SampleCache
{
    public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, object> Cache =
        new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
}

