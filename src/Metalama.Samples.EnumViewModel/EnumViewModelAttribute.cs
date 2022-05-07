// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

public class EnumViewModelAttribute : TypeAspect
{
    private static readonly DiagnosticDefinition<INamedType> _missingFieldError = 
        new DiagnosticDefinition<INamedType>( "ENUM01", Severity.Error, "The [EnumViewModel] aspect requires the type '{0}' to have a field named '_value'." );

    private static readonly SuppressionDefinition _suppression = new( "IDE0052" );
    
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var valueField = builder.Target.Fields.OfName( "_value" ).FirstOrDefault();

        if ( valueField == null )
        {
            // If the field does not exist, emit an error.
            builder.Diagnostics.Report( _missingFieldError.WithArguments( builder.Target ) );
            return;
        }

        // Suppress the IDE0052 warning telling that the field is assigned but not read. We get this at design time
        // because the partial class does not contain the member implementations.
        builder.Diagnostics.Suppress( _suppression, valueField );

        // Get the field type and decides the template.
        var enumType = (INamedType) valueField.Type;
        var isFlags = enumType.Attributes.Any( a => a.Type.Is( typeof( FlagsAttribute ) ) );
        var template = isFlags ? nameof( this.IsFlagTemplate ) : nameof( this.IsMemberTemplate );

        // Introduce a property into the view-model type for each enum member.
        foreach ( var member in enumType.Fields )
        {
            var propertyBuilder = builder.Advice.IntroduceProperty(
                builder.Target,
                template, 
                tags: new { member = member } );
            propertyBuilder.Name = "Is" + member.Name;
        }
    }

    // Template for the non-flags enum member.
    [Template]
    public bool IsMemberTemplate => meta.This._value == ((IField) meta.Tags["member"]!).Invokers.Final.GetValue( null);

    // Template for a flag enum member.
    [Template]
    public bool IsFlagTemplate
    {
        get
        {
            var field = (IField) meta.Tags["member"]!;

            // Note that the next line does not work for the "zero" flag, but currently Metalama does not expose the constant value of the enum
            // member so we cannot test its value at compile time.
            return (meta.This._value & field.Invokers.Final.GetValue( null )) == ((IField) meta.Tags["member"]!).Invokers.Final.GetValue( null );
        }
    }
}
