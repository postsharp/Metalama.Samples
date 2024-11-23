using Metalama.Framework.Code;

namespace Metalama.Samples.AbstractFactory;

internal sealed class FactoryComponentAnnotation : IAnnotation<INamedType>
{
    public FactoryComponentAnnotation(INamedType implementationType)
    {
        this.ImplementationType = implementationType;
    }

    public INamedType ImplementationType { get; }
}