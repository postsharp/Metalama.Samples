using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Project;

[Inheritable]
[EditorExperience( SuggestAsLiveTemplate = true )]
public class CloneableAttribute : TypeAspect
{
    private static readonly DiagnosticDefinition<(DeclarationKind, IFieldOrProperty)> _fieldOrPropertyCannotBeReadOnly =  /*<DiagnosticDefinitions>*/
        new( "CLONE01", Severity.Error, "The {0} '{1}' cannot be read-only because it is marked as a [Child]." );

    private static readonly DiagnosticDefinition<(DeclarationKind, IFieldOrProperty, IType)> _missingCloneMethod = 
        new( "CLONE02", Severity.Error, "The {0} '{1}' cannot be a [Child] because its type '{2}' does not have a 'Clone' parameterless method." );

    private static readonly DiagnosticDefinition<IProperty> _childPropertyMustBeAutomatic =
        new( "CLONE03", Severity.Error, "The property '{0}' cannot be a [Child] because is not an automatic property." );  /*</DiagnosticDefinitions>*/

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Verify that child fields are valid.
        var hasError = false;               /*<Verify>*/
        foreach ( var fieldOrProperty in GetClonableFieldsOrProperties( builder.Target ) )
        {
            // The field or property must be writable.
            if ( fieldOrProperty.Writeability != Writeability.All )
            {
                builder.Diagnostics.Report( _fieldOrPropertyCannotBeReadOnly.WithArguments( (fieldOrProperty.DeclarationKind, fieldOrProperty) ), fieldOrProperty );
                hasError = true;
            }

            // If it is a field, it must be an automatic property.
            if ( fieldOrProperty is IProperty property && property.IsAutoPropertyOrField == false )
            {
                builder.Diagnostics.Report( _childPropertyMustBeAutomatic.WithArguments( property ), property );
                hasError = true;
            }

            // The type of the field must be cloneable.
            if ( !MetalamaExecutionContext.Current.ExecutionScenario.IsDesignTime )
            { 
                var fieldType = fieldOrProperty.Type as INamedType;

                if ( fieldType == null ||
                    !(fieldType.AllMethods.OfName( "Clone" ).Where( p => p.Parameters.Count == 0 ).Any() ||
                    ( fieldType.BelongsToCurrentProject && fieldType.Enhancements().HasAspect<CloneableAttribute>()) ) )
                {
                    builder.Diagnostics.Report( _missingCloneMethod.WithArguments( (fieldOrProperty.DeclarationKind, fieldOrProperty, fieldOrProperty.Type) ), fieldOrProperty );
                    hasError = true;
                }
            }
        }

        // Stop here if we have errors.
        if ( hasError )
        {
            builder.SkipAspect();
            return;
        }  /*</Verify>*/

        // Introduce the Clone method.
        builder.Advice.IntroduceMethod(
            builder.Target,
            nameof(this.CloneImpl),
            whenExists: OverrideStrategy.Override,
            args: new { T = builder.Target },
            buildMethod: m =>
            {
                m.Name = "Clone";
                m.ReturnType = builder.Target;
            } );

        // Implement the ICloneable interface.
        builder.Advice.ImplementInterface(
            builder.Target,
            typeof(ICloneable),
            OverrideStrategy.Ignore );
    }

    private static IEnumerable<IFieldOrProperty> GetClonableFieldsOrProperties( INamedType type )
        => type.FieldsAndProperties.Where( f => f.Attributes.OfAttributeType( typeof( ChildAttribute ) ).Any() );

    [Template]
    public virtual T CloneImpl<[CompileTime] T>()
    {
        // This compile-time variable will receive the expression representing the base call.
        // If we have a public Clone method, we will use it (this is the chaining pattern). Otherwise,
        // we will call MemberwiseClone (this is the initialization of the pattern).
        IExpression baseCall;

        if ( meta.Target.Method.IsOverride )
        {
            baseCall = (IExpression) meta.Base.Clone();
        }
        else
        {
            baseCall = (IExpression) meta.This.MemberwiseClone();
        }

        // Define a local variable of the same type as the target type.
        var clone = (T) baseCall.Value!;

        // Select clonable fields.
        var clonableFields = GetClonableFieldsOrProperties( meta.Target.Type );

        foreach ( var field in clonableFields )
        {
            // Check if we have a public method 'Clone()' for the type of the field.
            var fieldType = (INamedType) field.Type;

            field.With( clone ).Value = meta.Cast( fieldType, field.Value?.Clone() );
        }

        return clone;
    }

    [InterfaceMember( IsExplicit = true )]
    private object Clone() => meta.This.Clone();
}