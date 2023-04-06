using Metalama.Framework.Aspects;

[assembly: AspectOrder( typeof(CacheAttribute), typeof(GenerateCacheKeyAttribute), typeof(CacheKeyMemberAttribute))]