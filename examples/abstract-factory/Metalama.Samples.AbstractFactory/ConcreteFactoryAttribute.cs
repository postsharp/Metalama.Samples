using System.Diagnostics;
using System.Linq;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;

namespace Metalama.Samples.AbstractFactory;

public class ConcreteFactoryAttribute : TypeAspect
{
    private readonly Type _interfaceType;

    public ConcreteFactoryAttribute(Type interfaceType)
    {
        this._interfaceType = interfaceType;
    }

    public bool WithImplementationsFromCurrentNamespace { get; init; }
    public string[] WithImplementationsFromNamespaces { get; init; } = [];

    private IEnumerable<FactoryComponentRegistration> GetRegistrations( INamedType targetType )
    {
        var registrations = targetType.Enhancements().GetAnnotations<FactoryComponentRegistration>();

        if (this.WithImplementationsFromCurrentNamespace)
        {
            registrations = registrations.Concat(targetType.ContainingNamespace.Types.Select( t => new FactoryComponentRegistration(t) ));
        }

        if ( this.WithImplementationsFromNamespaces != null && this.WithImplementationsFromNamespaces.Length > 0 )
        {
            registrations = registrations.Concat(this.WithImplementationsFromNamespaces.Select(ns => targetType.Compilation.GlobalNamespace.GetDescendant(ns)).WhereNotNull().SelectMany(ns => ns.Types.Select(t => new FactoryComponentRegistration(t))));
        }


        return registrations;
    }

    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        base.BuildAspect(builder);

        var interfaceType = (INamedType)TypeFactory.GetType(this._interfaceType);
        var regisrations = this.GetRegistrations(builder.Target);

        // Map the implementation types.
        var implementationTypeDictionary = regisrations.SelectMany(a =>
                a.ImplementationType.AllImplementedInterfaces.Select(i =>
                    (Annotation: a, Interface: i)))
            .GroupBy(x => x.Interface)
            .ToDictionary( g => g.Key, g => g.ToArray(), builder.Target.Compilation.Comparers.Default);
        
     
        // Iterate factory methods.
        foreach (var interfaceMethod in interfaceType.Methods.OrderBy(m => m.Name))
        {
            // Determine if the type already implements the method.
            if (builder.Target.TryFindImplementationForInterfaceMember(interfaceMethod, out _))
            {
                // The interface method is explicitly implemented by the type, so we don't need to generate it.
                continue;
            }
            
            // Find the implementation type.
            INamedType implementationType;

            if ( interfaceMethod.ReturnType is not INamedType returnType || !implementationTypeDictionary.TryGetValue( returnType, out var implementationTypes))
            {
                builder.Diagnostics.Report(DiagnosticDefinitions.NoImplementationType.WithArguments((interfaceMethod.ReturnType, interfaceMethod)));
                continue;
            }

            // Give priority to implementation types where the method name is explicitly specified.
            var implementationTypesWithExplicitlySetMethod = implementationTypes.Where(x => x.Annotation.MethodName == interfaceMethod.Name).ToList();

            switch (implementationTypesWithExplicitlySetMethod.Count)
            {
                case 0:
                    // Fall back to implementation types where the method name is not specified.
                    var implementationTypesWithoutExplicitlySetMethod = implementationTypes.Where(x => x.Annotation.MethodName == null).ToList();

                    switch (implementationTypesWithoutExplicitlySetMethod.Count)
                    {
                        case 0:
                            builder.Diagnostics.Report(DiagnosticDefinitions.NoImplementationType.WithArguments((interfaceMethod.ReturnType, interfaceMethod)));
                            continue;
                        
                        case 1:
                            implementationType = implementationTypesWithoutExplicitlySetMethod[0].Annotation.ImplementationType;
                            break;
                        
                        default:
                            ReportAmbiguousMatch(implementationTypesWithoutExplicitlySetMethod.Select(x=>x.Annotation.ImplementationType));
                            continue;
                    }

                    break;
                    
                case 1:
                    implementationType = implementationTypesWithExplicitlySetMethod[0].Annotation.ImplementationType;
                    break;
                
                default:
                    ReportAmbiguousMatch(implementationTypesWithExplicitlySetMethod.Select(x=>x.Annotation.ImplementationType));
                    continue;
            }

            

            // Find a constructor matching the interface method signature.
            var implementationConstructor =
                implementationType.Constructors.SingleOrDefault(c =>
                    c.Parameters.Select(p => p.Type)
                        .SequenceEqual(interfaceMethod.Parameters.Select(p => p.Type)));

            if (implementationConstructor == null)
            {
                builder.Diagnostics.Report(DiagnosticDefinitions.NoConstructor.WithArguments((implementationType, interfaceMethod)));
                continue;
            }

            // Introduce the method.
            builder.IntroduceMethod(
                nameof(this.CreateMember),
                args: new { implementationConstructor },
                buildMethod: m =>
                {
                    m.ReturnType = interfaceMethod.ReturnType;
                    m.Name = interfaceMethod.Name;

                    foreach (var parameter in interfaceMethod.Parameters)
                    {
                        m.AddParameter(parameter.Name, parameter.Type);
                    }
                });
            
            // Local function that reports an error for ambiguous match.
            void ReportAmbiguousMatch( IEnumerable<INamedType> ambiguousTypes )
            {
                builder.Diagnostics.Report(
                    DiagnosticDefinitions.AmbiguousImplementationType.WithArguments((
                        interfaceMethod.ReturnType, interfaceMethod,
                        string.Join(", ",
                            ambiguousTypes.Select(t => $"'{t.Name}'")))));
            }
        }
    }

    [Template]
    public dynamic CreateMember(IConstructor implementationConstructor)
        => implementationConstructor.Invoke( meta.Target.Parameters )!;
}