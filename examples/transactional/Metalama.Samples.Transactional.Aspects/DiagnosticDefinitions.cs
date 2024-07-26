using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Samples.Transactional.Aspects;

[CompileTime]
internal static class DiagnosticDefinitions
{
    public static DiagnosticDefinition<INamedType> BaseTypeHasNoStateType
        = new( "STATE01", Severity.Error,
            "The base type '{0}' does not have a 'State' nested type." );
    
    public static DiagnosticDefinition<INamedType> StateTypeMustBeProtected
        = new( "STATE02", Severity.Error,
            "The type '{0}' must be protected." );
    
    public static DiagnosticDefinition<INamedType> StateTypeMustNotBeSealed
        = new( "STATE03", Severity.Error,
            "The type '{0}' must be not be sealed." );
    
    public static DiagnosticDefinition<INamedType> StateTypeMustHaveConstructor
        = new( "STATE04", Severity.Error,
            "The type '{0}' must have a constructor with a single parameter of type TransactionalObjectId." );
    
    public static DiagnosticDefinition<IConstructor> StateConstructorMustBePublicOrProtected
        = new( "STATE05", Severity.Error,
            "The constructor '{0}' must be public or protected." );
    
    public static DiagnosticDefinition<INamedType> TypeMustBeTransactionalObject
        = new( "STATE06", Severity.Error,
            "The type '{0}' must be derived from TransactionalObject." );
}