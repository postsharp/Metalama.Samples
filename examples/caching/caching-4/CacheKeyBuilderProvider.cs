internal class CacheKeyBuilderProvider : ICacheKeyBuilderProvider
{
    public ICacheKeyBuilder<TValue> GetCacheKeyBuilder<TValue, TBuilder>( in TValue value )
        where TBuilder : ICacheKeyBuilder<TValue>, new()
    {
        return Instance<TBuilder>.Value;
    }

    private class Instance<TBuilder>
        where TBuilder : new()
    {
        public static TBuilder Value { get; } = new TBuilder();
    }
}