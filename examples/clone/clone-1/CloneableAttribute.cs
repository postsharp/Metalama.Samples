using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

[Inheritable]
[EditorExperience( SuggestAsLiveTemplate = true )]
public class CloneableAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.Advice.ImplementInterface( /*<BuildAspect1>*/
            builder.Target,
            typeof(ICloneable),
            OverrideStrategy.Ignore ); /*</BuildAspect1>*/

        builder.Advice.IntroduceMethod( /*<BuildAspect2>*/
            builder.Target,
            nameof(this.CloneImpl),
            whenExists: OverrideStrategy.Override,
            args: new { T = builder.Target },
            buildMethod: m => m.Name = "Clone" ); /*</BuildAspect2>*/
    }

    [InterfaceMember( IsExplicit = true )]
    private object Clone() => meta.This.Clone();

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
        var clonableFields =
            meta.Target.Type.FieldsAndProperties.Where(
                f => f.Attributes.OfAttributeType( typeof(ChildAttribute) ).Any() );

        foreach ( var field in clonableFields )
        {
            // Check if we have a public method 'Clone()' for the type of the field.
            var fieldType = (INamedType) field.Type;

            field.With( clone ).Value = meta.Cast( fieldType, field.Value?.Clone() );
        }

        return clone;
    }
}