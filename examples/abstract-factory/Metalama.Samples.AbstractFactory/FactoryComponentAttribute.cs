using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Samples.AbstractFactory;

public sealed class FactoryComponentAttribute : TypeAspect
{
    private readonly Type _factoryType;

    public FactoryComponentAttribute(Type factoryType)
    {
        this._factoryType = factoryType;
    }

    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        builder.With( (INamedType) TypeFactory.GetType(this._factoryType)).AddAnnotation(new FactoryComponentAnnotation(builder.Target));
    }
}