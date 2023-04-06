using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

public class CacheKeyMemberAttribute : FieldOrPropertyAspect
{
    public override void BuildAspect( IAspectBuilder<IFieldOrProperty> builder )
    {
        builder.Outbound.Select( f => f.DeclaringType ).RequireAspect<GenerateCacheKeyAttribute>();
    }
}