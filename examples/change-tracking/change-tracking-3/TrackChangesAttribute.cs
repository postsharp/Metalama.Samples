using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

#pragma warning disable IDE0031, IDE1005

[Inheritable]
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

    private static readonly DiagnosticDefinition<IMethod> _onPropertyChangedMustBeVirtual = new(
        "MY003",
        Severity.Error,
        "The '{0}' method must be virtual.");

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Implement the ISwitchableChangeTracking interface.         
        var implementInterfaceResult = builder.Advice.ImplementInterface( builder.Target,
            typeof(ISwitchableChangeTracking), OverrideStrategy.Ignore );

        if ( implementInterfaceResult.Outcome == AdviceOutcome.Ignore )
        {
            // If the type already implements ISwitchableChangeTracking, it must have a protected method called OnChanged, without parameters, otherwise
            // this is a contract violation, so we report an error.

            var onChangeMethod = builder.Target.AllMethods.OfName( nameof(this.OnChange) )
                .SingleOrDefault( m => m.Parameters.Count == 0 );

            if ( onChangeMethod == null )
            {
                builder.Diagnostics.Report( _mustHaveOnChangeMethod.WithArguments( builder.Target ) );
            }
            else if ( onChangeMethod.Accessibility != Accessibility.Protected )
            {
                builder.Diagnostics.Report( _onChangeMethodMustBeProtected );
            }
        }
        else
        {
            builder.Advice.IntroduceField( builder.Target, "_isTrackingChanges", typeof(bool) );
        }


        var onPropertyChanged = this.GetOnPropertyChangedMethod( builder.Target );

        if ( onPropertyChanged == null ) /*<NoOnPropertyChanged>*/
        {
            // If the type has an OnPropertyChanged method, we assume that all properties
            // and fields already call it, and we hook into OnPropertyChanged instead of
            // overriding each setter.

            var fieldsOrProperties = builder.Target.FieldsAndProperties
                .Where( f =>
                    !f.IsImplicitlyDeclared && f.Writeability == Writeability.All && f.IsAutoPropertyOrField == true );

            foreach ( var fieldOrProperty in fieldsOrProperties )
            {
                builder.Advice.OverrideAccessors( fieldOrProperty, null, nameof(this.OverrideSetter) );
            }
        } /*</NoOnPropertyChanged>*/
        else if ( onPropertyChanged.DeclaringType.Equals( builder.Target ) ) /*<OnPropertyChangedInCurrentType>*/
        {
            // If the OnPropertyChanged method was declared in the current type, override it.
            builder.Advice.Override( onPropertyChanged, nameof(this.OnPropertyChanged) );
        } /*</OnPropertyChangedInCurrentType>*/
        else if ( implementInterfaceResult.Outcome == AdviceOutcome.Ignore ) /*<OnPropertyChangedInBaseType>*/
        {
            // If we have an OnPropertyChanged method but the type already implements ISwitchableChangeTracking,
            // we assume that the type already hooked the OnPropertyChanged method, and
            // there is nothing else to do.
        }
        else
        {
            // If the OnPropertyChanged method was defined in a base class, but not overridden
            // in the current class, and if we implement ISwitchableChangeTracking ourselves,
            // then we need to override OnPropertyChanged.

            if ( !onPropertyChanged.IsVirtual )
            {
                builder.Diagnostics.Report( _onPropertyChangedMustBeVirtual.WithArguments( onPropertyChanged ) );
            }
            else
            {
                builder.Advice.IntroduceMethod( builder.Target, nameof(this.OnPropertyChanged),
                    whenExists: OverrideStrategy.Override );
            }
        } /*</OnPropertyChangedInBaseType>*/
    }


    private IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods
            .OfName( "OnPropertyChanged" )
            .SingleOrDefault( m => m.Parameters.Count == 1 );

    [InterfaceMember] public bool IsChanged { get; private set; }


    [InterfaceMember]
    public bool IsTrackingChanges
    {
        get => meta.This._isTrackingChanges;
        set
        {
            if ( meta.This._isTrackingChanges != value )
            {
                meta.This._isTrackingChanges = value;

                var onPropertyChanged = this.GetOnPropertyChangedMethod( meta.Target.Type );

                if ( onPropertyChanged != null )
                {
                    onPropertyChanged.Invoke( nameof(this.IsTrackingChanges) );
                }
            }
        }
    }

    [InterfaceMember]
    public void AcceptChanges() => this.IsChanged = false;


    [Introduce( WhenExists = OverrideStrategy.Ignore )]
    protected void OnChange()
    {
        if ( this.IsChanged == false )
        {
            this.IsChanged = true;

            var onPropertyChanged = this.GetOnPropertyChangedMethod( meta.Target.Type );

            if ( onPropertyChanged != null )
            {
                onPropertyChanged.Invoke( nameof(this.IsChanged) );
            }
        }
    }

    [Template]
    private void OverrideSetter( dynamic? value )
    {
        meta.Proceed();

        if ( value != meta.Target.Property.Value )
        {
            this.OnChange();
        }
    }

    [Template]
    protected virtual void OnPropertyChanged( string name )
    {
        meta.Proceed();

        if ( name is not (nameof(this.IsChanged) or nameof(this.IsTrackingChanges)) )
        {
            this.OnChange();
        }
    }
}