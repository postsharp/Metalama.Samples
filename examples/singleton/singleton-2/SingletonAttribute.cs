using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

[assembly: AspectOrder( "SingletonAttribute+SingletonConstructorAttribute", "SingletonAttribute" )]

public class SingletonAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.Outbound.SelectMany( t => t.Constructors ).AddAspect<SingletonConstructorAttribute>();
    }

    private class SingletonConstructorAttribute : CanOnlyBeUsedFromAttribute
    {
        public SingletonConstructorAttribute()
        {
            // Allow from test namespaces.
            this.Namespaces = ["**.Tests"];

            // Allow from the Startup class.
            this.Types = [typeof( Startup )];

            // Justification.
            this.Description = "The class is a [Singleton].";
        }
    }
}