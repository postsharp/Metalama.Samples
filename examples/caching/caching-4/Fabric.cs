using Metalama.Framework.Fabrics;

public class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        var cachingOptions = CachingOptions.Default
            .UseToString( typeof(int) )
            .UseToString( typeof(long) )
            .UseToString( typeof(string) )
            .UseCacheKeyBuilder( typeof(byte[]), typeof(ByteArrayCacheKeyBuilder) );

        amender.SetOptions( cachingOptions );
    }
}