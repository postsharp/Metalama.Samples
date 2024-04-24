using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

public class SingletonAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.Outbound
            .SelectMany( type => type.Constructors )
            .AddAspect( _ => new CanOnlyBeUsedFromAttribute { Namespaces = new[] { "**.Tests" } } );
    }
}