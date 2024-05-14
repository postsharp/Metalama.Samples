using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Aspects;

[CompileTime]
public class SingletonAttribute : CanOnlyBeUsedFromAttribute
{
    public SingletonAttribute()
    {
        // Allow from test namespaces.
        this.Namespaces = ["**.Tests"];
                
        // Allow from the Startup class.
        this.Types = [typeof(Startup)];
                
        // Justification.
        this.Description = $"The class is a [Singleton].";
    }
}
