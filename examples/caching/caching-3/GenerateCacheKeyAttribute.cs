using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Text;

public class GenerateCacheKeyAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.Advice.ImplementInterface( builder.Target, typeof(ICacheKey), whenExists: OverrideStrategy.Ignore );
    }

    [Introduce( WhenExists = OverrideStrategy.Override)]
    protected virtual void BuildCacheKey( StringBuilder stringBuilder )
    {
        // Call the base method, if any.

        if ( meta.Target.Method.IsOverride )
        {
            meta.Proceed();
            stringBuilder.Append( ", " );
        }

        // Select all cache key members.
        var members =
            meta.Target.Type.FieldsAndProperties
                .Where( f => f.Enhancements().HasAspect<CacheKeyMemberAttribute>() )
                .OrderBy( f => f.Name )
                .ToList();


        // This is how we define a compile-time variable of value 0.

        var i = meta.CompileTime(0);
        foreach ( var member in members )
        {            
            if ( i > 0 )
            {
                stringBuilder.Append( ", " );
            }

            i++;

            if ( member.Type.Is( typeof( ICacheKey ) ) || (member.Type is INamedType namedType && namedType.Enhancements().HasAspect<GenerateCacheKeyAttribute>()) )
            {
                if ( member.Type.IsNullable == false )
                {
                    stringBuilder.Append( member.Value!.ToCacheKey() );
                }
                else
                {
                    stringBuilder.Append( member.Value?.ToCacheKey() ?? "null" );
                }
            }
            else
            {
                if ( member.Type.IsNullable == false )
                {
                    stringBuilder.Append( member.Value );
                }
                else
                {
                    stringBuilder.Append( member.Value?.ToString() ?? "null" );
                }
            }
        }

    }

    [InterfaceMember]
    public string ToCacheKey()
    {
        var stringBuilder = new StringBuilder();
        this.BuildCacheKey( stringBuilder );
        

        return stringBuilder.ToString();


    }
}