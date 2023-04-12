using Metalama.Framework.Aspects;
using Metalama.Framework.Project;

[CompileTime]
public static class CachingProjectExtensions
{
    public static CachingOptions CachingOptions( this IProject project ) => project.Extension<CachingOptions>();
}
