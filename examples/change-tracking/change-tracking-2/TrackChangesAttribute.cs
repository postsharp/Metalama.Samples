using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;


public class TrackChangesAttribute : TypeAspect
{
    private static readonly DiagnosticDefinition<INamedType> _mustHaveOnChangeMethod = new(
        "MY001",
        Severity.Error,
        $"The '{nameof(ISwitchableChangeTracking)}' interface is implemented manually on type '{{0}}', but the type does not have an '{nameof(OnChange)}()' method.");

    private static readonly DiagnosticDefinition _onChangeMethodMustBeProtected = new(
        "MY002",
        Severity.Error,
        $"The '{nameof(OnChange)}()' method must be have the 'protected' accessibility.");

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Implement the ISwitchableChangeTracking interface.         
        var implementInterfaceResult = builder.Advice.ImplementInterface( builder.Target,
            typeof(ISwitchableChangeTracking), OverrideStrategy.Ignore ); /*[BuildAspect:Start]*/

        // If the type already implements IChangeTracking, it must have a protected method called OnChanged, without parameters, otherwise
        // this is a contract violation, so we report an error.
        if ( implementInterfaceResult.Outcome == AdviceOutcome.Ignored )
        {
            var onChangeMethod = builder.Target.AllMethods.OfName( nameof(this.OnChange) )
                .Where( m => m.Parameters.Count == 0 ).SingleOrDefault();

            if ( onChangeMethod == null )
            {
                builder.Diagnostics.Report( _mustHaveOnChangeMethod.WithArguments( builder.Target ) );
            }
            else if ( onChangeMethod.Accessibility != Accessibility.Protected )
            {
                builder.Diagnostics.Report( _onChangeMethodMustBeProtected );
            }
        }
        /*[BuildAspect:End]*/

        // Override all writable fields and automatic properties.
        var fieldsOrProperties = builder.Target.FieldsAndProperties
            .Where( f =>
                !f.IsImplicitlyDeclared && f.Writeability == Writeability.All && f.IsAutoPropertyOrField == true );

        foreach ( var fieldOrProperty in fieldsOrProperties )
        {
            builder.Advice.OverrideAccessors( fieldOrProperty, null, nameof(this.OverrideSetter) );
        }
    }


    [InterfaceMember]
    public bool IsChanged { get; private set; }

    [InterfaceMember]
    public bool IsTrackingChanges { get; set; }


    [InterfaceMember]
    public void AcceptChanges() => this.IsChanged = false;

    [Introduce( WhenExists = OverrideStrategy.Ignore )]
    protected void OnChange()
    {
        if ( this.IsTrackingChanges )
        {
            this.IsChanged = true;
        }
    }

    [Template]
    private void OverrideSetter( dynamic? value )
    {
        if ( value != meta.Target.Property.Value )
        {
            meta.Proceed();

            this.OnChange();
        }
    }
}