using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;


public class CacheAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        // Builds the caching string.
        var parameters = new object[target.Method.Parameters.Count];
        var stringBuilder = compileTime(new StringBuilder());
        stringBuilder.Append(target.Type.ToString());
        stringBuilder.Append('.');
        stringBuilder.Append(target.Method.Name);
        stringBuilder.Append('(');
        int i = compileTime(0);
        foreach (var p in target.Parameters)
        {
            string comma = i > 0 ? ", " : "";

            if (p.IsOut)
            {
                stringBuilder.Append($"{comma}{p.Name} = <out> ");
            }
            else
            {
                stringBuilder.Append($"{comma}{{{i}}}");
                parameters[i] = p.Value;
            }

            i++;
        }
        stringBuilder.Append(')');

        string cacheKey = string.Format(stringBuilder.ToString(), parameters);

        // Cache lookup.
        if (SampleCache.Cache.TryGetValue(cacheKey, out object value))
        {
            // Cache hit.
            return value;
        }
        else
        {
            // Cache miss. Go and invoke the method.
            dynamic result = proceed();

            // Add to cache.
            SampleCache.Cache.TryAdd(cacheKey, result);
            return result;
        }
    }
}

// Placeholder implementation of a cache because the hosted try.postsharp.net does not allow for MemoryCache.
public static class SampleCache
{
    public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, object> Cache =
        new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
}