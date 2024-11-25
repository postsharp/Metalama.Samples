using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Samples.AbstractFactory;

[CompileTime]
internal static class DiagnosticDefinitions
{
    public static readonly DiagnosticDefinition<(IType,IMethod)> NoImplementationType =
        new(
            "FACTORY01", Severity.Error,
            "No implementation type was found implementing the return type '{0}' of the '{1}' method.");
    
    public static readonly DiagnosticDefinition<(IType,IMethod,string)> AmbiguousImplementationType =
        new(
            "FACTORY02", Severity.Error,
            "Several types implement the return type '{0}' of the '{1}' method: {2}.");
    
    public static readonly DiagnosticDefinition<(INamedType,IMethod)> NoConstructor =
        new( "FACTORY03", Severity.Error,
            "The '{0}' type doesn't have a constructor that has the same parameters as the '{1}' method.");
}