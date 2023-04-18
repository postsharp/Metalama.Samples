using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using System.ComponentModel;

namespace Metalama.Samples.Dirty;


public class TrackChangesAttribute : TypeAspect
{
    private static readonly DiagnosticDefinition<INamedType> _mustHaveOnChangeMethod = new(
        "MY001",
        Severity.Error,
        $"The '{nameof( ISwitchableChangeTracking )}' interface is implemented manually on type '{{0}}', but the type does not have an '{nameof( OnChange )}()' method." );

    private static readonly DiagnosticDefinition _onChangeMethodMustBeProtected = new(
        "MY002",
        Severity.Error,
        $"The '{nameof( OnChange )}()' method must be have the 'protected' accessibility." );

    public bool IsRevertible { get; set; }

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Select fields and automatic properties that can be changed.
        var fieldsOrProperties = builder.Target.FieldsAndProperties
        .Where( f => !f.IsImplicitlyDeclared && f.Writeability == Writeability.All && f.IsAutoPropertyOrField == true );


        var introducedFields = new Dictionary<IFieldOrProperty, IField>();   /*[BuildDictionary:Start]*/

        if ( this.IsRevertible )
        {
         
            // Create a field for each mutable field or property. These fields
            // will contain the accepted values.
            foreach ( var fieldOrProperty in fieldsOrProperties )
            {
                var upperCaseName = fieldOrProperty.Name.TrimStart( '_' );
                upperCaseName = upperCaseName.Substring( 0, 1 ).ToUpper() + upperCaseName.Substring( 1 );
                var acceptedField = builder.Advice.IntroduceField( builder.Target, "_accepted" + upperCaseName, fieldOrProperty.Type );
                introducedFields[fieldOrProperty] = acceptedField.Declaration;
            }
        } 

        // Implement the ISwitchableChangeTracking interface.         
        var implementInterfaceResult = builder.Advice.ImplementInterface( builder.Target, typeof( ISwitchableChangeTracking ), OverrideStrategy.Ignore,
            tags: new { IntroducedFields = introducedFields } ); /*[BuildDictionary:End]*/

        // If the type already implements ISwitchableChangeTracking, it must have a protected method called OnChanged, without parameters, otherwise
        // this is a contract violation, so we report an error.
        if ( implementInterfaceResult.Outcome == AdviceOutcome.Ignored )
        {
            var onChangeMethod = builder.Target.AllMethods.OfName( nameof( OnChange ) ).Where( m => m.Parameters.Count == 0 ).SingleOrDefault();

            if ( onChangeMethod == null )
            {
                builder.Diagnostics.Report( _mustHaveOnChangeMethod.WithArguments( builder.Target ) );
            }
            else if ( onChangeMethod.Accessibility != Accessibility.Protected )
            {
                builder.Diagnostics.Report( _onChangeMethodMustBeProtected );
            }
        }

        if ( this.IsRevertible ) /*[IRevertibleChangeTracking:Start]*/
        {
            builder.Advice.ImplementInterface( builder.Target, typeof(IRevertibleChangeTracking), OverrideStrategy.Ignore,
            tags: new { IntroducedFields = introducedFields } );
        }  /*[IRevertibleChangeTracking:End]*/

        // Override all writable fields and automatic properties.
        // If the type has an OnPropertyChanged method, we assume that all properties
        // and fields already call it, and we hook into OnPropertyChanged instead of
        // overriding each setter.
        var onPropertyChanged = this.GetOnPropertyChangedMethod( builder.Target );

        if ( onPropertyChanged == null )
        {

            foreach ( var fieldOrProperty in fieldsOrProperties )
            {
                builder.Advice.OverrideAccessors( fieldOrProperty, null, nameof( this.OverrideSetter ) );
            }
        }

    }

    [Introduce]
    private bool _isTrackingChanges;


    [InterfaceMember]
    public bool IsChanged { get; private set; }

    [InterfaceMember]
    public bool IsTrackingChanges
    {
        get => this._isTrackingChanges;
        set
        {
            if ( this._isTrackingChanges != value )
            {
                this._isTrackingChanges = value;

                var onPropertyChanged = this.GetOnPropertyChangedMethod( meta.Target.Type );

                if ( onPropertyChanged != null )
                {
                    onPropertyChanged.Invoke( nameof( this.IsTrackingChanges ) );
                }
            }
        }
    }

    [InterfaceMember]
    public void AcceptChanges()
    {
        if ( this.IsRevertible )
        {

            var introducedFields = (Dictionary<IFieldOrProperty, IField>) meta.Tags["IntroducedFields"]!;

            foreach ( var field in introducedFields )
            {
                field.Value.Value = field.Key.Value;
            }
        }

        this.IsChanged = false;

    }

    [InterfaceMember]
    public void RejectChanges()
    {
        
        var introducedFields = (Dictionary<IFieldOrProperty, IField>) meta.Tags["IntroducedFields"]!;

        foreach ( var field in introducedFields )
        {
            field.Key.Value = field.Value.Value;
        }

        this.IsChanged = false;

    }

    private IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods
                .OfName( "OnPropertyChanged" )
                .Where( m => m.Parameters.Count == 1 )
                .SingleOrDefault();


    [Introduce( WhenExists = OverrideStrategy.Ignore )]
    protected void OnChange()
    {
        if ( this.IsChanged == false )
        {
            this.IsChanged = true;

            var onPropertyChanged = this.GetOnPropertyChangedMethod( meta.Target.Type );

            if ( onPropertyChanged != null )
            {
                onPropertyChanged.Invoke( nameof( this.IsChanged ) );
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
}