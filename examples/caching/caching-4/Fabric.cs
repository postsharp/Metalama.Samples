using Metalama.Framework.Fabrics;

public class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Project.CachingOptions().UseToString( typeof( int ) );
        amender.Project.CachingOptions().UseToString( typeof( long ) );
        amender.Project.CachingOptions().UseToString( typeof( string ) );
        amender.Project.CachingOptions().UseCacheKeyBuilder( typeof( byte[] ), typeof( ByteArrayCacheKeyBuilder ) );
    }
}
