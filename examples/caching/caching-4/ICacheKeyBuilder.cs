public interface ICacheKeyBuilder<T>
{
    public string? GetCacheKey( in T value, ICacheKeyBuilderProvider  provider );
}
