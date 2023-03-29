using System.Collections.Concurrent;

// Placeholder implementation of a cache because the hosted try.metalama.net does not allow for MemoryCache.
public static class SampleCache
{
    public static readonly ConcurrentDictionary<string, object> Cache =
        new();
}