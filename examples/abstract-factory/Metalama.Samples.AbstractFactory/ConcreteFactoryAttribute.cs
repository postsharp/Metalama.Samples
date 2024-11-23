using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Samples.AbstractFactory;

public class ConcreteFactoryAttribute : TypeAspect
{
    private readonly Type _interfaceType;

    public ConcreteFactoryAttribute(Type interfaceType)
    {
        this._interfaceType = interfaceType;
    }

    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        base.BuildAspect(builder);

        var interfaceType = (INamedType) TypeFactory.GetType(this._interfaceType);
        var annotations = builder.Target.Enhancements().GetAnnotations<FactoryComponentAnnotation>().ToList();

        foreach (var method in interfaceType.Methods.OrderBy( m => m.Name ) )
        {
            // TODO: report errors in case of missing or ambiguous. implementation
            var implementationType = annotations.Single(a => a.ImplementationType.Is( method.ReturnType ));

            // TODO: report error if constructor cannot be fond.
            var implementationConstructor =
                implementationType.ImplementationType.Constructors.Single( c => c.Parameters.Count == 0);

            builder.IntroduceMethod(nameof(this.CreateMember),
                args: new { implementationConstructor },
                buildMethod: m =>
                {
                    m.ReturnType = method.ReturnType;
                    m.Name = method.Name;
                });
        }
    }

    [Template]
    public dynamic CreateMember(IConstructor implementationConstructor)
        => implementationConstructor.Invoke()!;
}