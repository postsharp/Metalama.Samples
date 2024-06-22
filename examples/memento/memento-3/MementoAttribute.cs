using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Sample;

public class MementoAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var mementoType =
            builder.IntroduceClass(
                "Memento",
                buildType: b =>
                {
                    b.Accessibility = Metalama.Framework.Code.Accessibility.Public;
                } );

        var mementoFields = new List<IField>();

        foreach ( var fieldOrProperty in builder.Target.Properties )
        {
            if ( fieldOrProperty is not { IsAutoPropertyOrField: true, IsImplicitlyDeclared: false } )
            {
                continue;
            }

            var field = mementoType.IntroduceField(
                nameof( this._mementoField ),
                buildField: b =>
                {
                    b.Name = fieldOrProperty.Name;
                    b.Type = fieldOrProperty.Type;
                    b.Accessibility = Metalama.Framework.Code.Accessibility.Public;
                    b.Writeability = Writeability.ConstructorOnly;
                } );

            mementoFields.Add( field.Declaration );
        }

        var originatorProperty =
            mementoType.IntroduceProperty(
                nameof( this.Originator ),
                buildProperty: b =>
                {
                    b.Name = "Originator";
                    b.Type = TypeFactory.GetType( typeof( IOriginator ) );
                    b.Accessibility = Metalama.Framework.Code.Accessibility.Public;
                } );

        mementoType.ImplementInterface( typeof( IMemento ) );

        mementoType.IntroduceConstructor(
            nameof( MementoConstructorTemplate ),
            buildConstructor: b =>
            {
                b.AddParameter( "originator", builder.Target );

                foreach ( var mementoField in mementoFields )
                {
                    b.AddParameter( mementoField.Name, mementoField.Type );
                }
            },
            args: new { originatorProperty = originatorProperty.Declaration, fields = mementoFields } );

        builder.ImplementInterface( typeof( IOriginator ), tags: new { mementoType = mementoType.Declaration } );
    }

    [Template]
#pragma warning disable IDE0044 // Add readonly modifier
    private object? _mementoField;
#pragma warning restore IDE0044 // Add readonly modifier

    [Template]
    public object? Originator { get; }

    [InterfaceMember]
    public IMemento Save()
    {
        var mementoType = (INamedType) meta.Tags["mementoType"]!;
        var fieldExpressions = meta.Target.Type.FieldsAndProperties.Where( f => f.IsAutoPropertyOrField == true && !f.IsImplicitlyDeclared );

        return mementoType.Constructors.Single().Invoke( fieldExpressions.Prepend((IExpression)meta.This) )!;
    }

    [InterfaceMember]
    public void Restore( IMemento memento )
    {
        var mementoType = (INamedType) meta.Tags["mementoType"]!;

        foreach ( var fieldOrProperty in meta.Target.Type.FieldsAndProperties.Where( f => f.IsAutoPropertyOrField == true && !f.IsImplicitlyDeclared ) )
        {
            var mementoField = mementoType.FieldsAndProperties.OfName( fieldOrProperty.Name ).Single();

            fieldOrProperty.Value = mementoField.With( (IExpression) meta.Cast( mementoType, memento )! ).Value;
        }
    }

    [Template]
    public void MementoConstructorTemplate( [CompileTime] IProperty originatorProperty, [CompileTime] List<IField> fields )
    {
        int i = meta.CompileTime( 0 );

        originatorProperty.Value = meta.Target.Constructor.Parameters.First();

        foreach ( var parameter in meta.Target.Constructor.Parameters.Skip( 1 ) )
        {
            fields[i].Value = parameter;
            i++;
        }
    }
}
