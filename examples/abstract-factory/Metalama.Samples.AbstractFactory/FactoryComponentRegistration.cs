using Metalama.Framework.Code;

namespace Metalama.Samples.AbstractFactory;

internal sealed class FactoryComponentRegistration : IAnnotation<INamedType>
{
    public FactoryComponentRegistration(INamedType implementationType, string? methodName = null)
    {
        this.ImplementationType = implementationType;
        this.MethodName = methodName;
    }

    public INamedType ImplementationType { get; }
    public string? MethodName { get; }
}