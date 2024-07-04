using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Sample;

public partial class MementoAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Introduce a new private nested class called Memento.
        var mementoType =
            builder.IntroduceClass(
                "Memento",
                buildType: b => b.Accessibility = Metalama.Framework.Code.Accessibility.Private );

        // Introduce originator property that will hold a reference to the instance that created the Memento.
        var originatorProperty =
            mementoType.IntroduceProperty(
                nameof( this.Originator ),
                buildProperty: b =>
                {
                    b.Name = "Originator";
                    b.Type = TypeFactory.GetType( typeof( IOriginator ) );
                    b.Accessibility = Metalama.Framework.Code.Accessibility.Public;
                } );

        // Dictionary that maps fields of the target class to memento properties.
        var mementoPropertyMap = new Dictionary<IFieldOrProperty, IProperty>();

        // Introduce data properties to the Memento class for each field of the target class.
        foreach ( var fieldOrProperty in builder.Target.FieldsAndProperties )
        {
            if ( fieldOrProperty is not { IsAutoPropertyOrField: true, IsImplicitlyDeclared: false } )
            {
                // Ignore properties that are not auto-properties and fields that are not explicitly declared.
                continue;
            }

            if ( fieldOrProperty.Writeability is not Writeability.All ||
                 fieldOrProperty.Attributes.OfAttributeType( typeof( MementoIgnoreAttribute ) ).Any() )
            {
                // Ignore read-only declarations and those marked with the MementoIgnore attribute.
                continue;
            }

            var introducedField = mementoType.IntroduceProperty(
                nameof( this.MementoProperty ),
                buildProperty : b =>
                {
                    var trimmedName = fieldOrProperty.Name.TrimStart( '_' );

                    b.Name = trimmedName.Substring( 0, 1 ).ToUpperInvariant() + trimmedName.Substring( 1 );
                    b.Type = fieldOrProperty.Type;
                } );

            mementoPropertyMap.Add( fieldOrProperty, introducedField.Declaration );
        }

        // Implement the IMemento interface on the Memento class.
        mementoType.ImplementInterface( typeof( IMemento ) );

        // Add a constructor to the Memento class that records the state of the .
        mementoType.IntroduceConstructor(
            nameof( MementoConstructorTemplate ),
            buildConstructor: b =>
            {
                b.AddParameter( "originator", builder.Target );
            },
            args: new { originatorProperty = originatorProperty.Declaration, propertyMap = mementoPropertyMap } );

        // Introduce a Restore method to the target class, that loads the state of the object from a Memento.
        builder.IntroduceMethod(
            nameof( Save ),
            args: new { mementoType = mementoType.Declaration } );

        // Introduce a Restore method to the target class, that loads the state of the object from a Memento.
        builder.IntroduceMethod(
            nameof( Restore ),
            args: new { mementoType = mementoType.Declaration, propertyMap = mementoPropertyMap } );

        // Implement the rest of the IOriginator interface.
        builder.ImplementInterface( typeof( IOriginator ) );
    }

    [Template]
    public object? MementoProperty { get; }

    [Template]
    public object? Originator { get; }

    [Template]
    public IMemento Save( [CompileTime] INamedType mementoType )
    {
        // Invoke the constructor of the Memento class and pass this object as the originator.
        return mementoType.Constructors.Single().Invoke( (IExpression) meta.This )!;
    }

    [Template]
    public void Restore( IMemento memento, [CompileTime] INamedType mementoType, [CompileTime] Dictionary<IFieldOrProperty, IProperty> propertyMap)
    {
        var onPropertyChangedMethod = meta.Target.Type.AllMethods.OfName( "OnPropertyChanged" ).Single( x => x.Parameters is [{ Type.SpecialType: SpecialType.String }] );

        // Set fields of this instance to the values stored in the Memento.
        foreach ( var pair in propertyMap )
        {
            var old = pair.Key.Value;
            pair.Key.Value = pair.Value.With( (IExpression) meta.Cast( mementoType, memento )! ).Value;
            onPropertyChangedMethod.Invoke( pair.Value.Name );
        }
    }

    [Template]
    public void MementoConstructorTemplate( [CompileTime] IProperty originatorProperty, [CompileTime] Dictionary<IFieldOrProperty, IProperty> propertyMap )
    {
        // Set the originator property and the data properties of the Memento.
        originatorProperty.Value = meta.Target.Parameters[0];

        foreach ( var pair in propertyMap )
        {
            pair.Value.Value = pair.Key.With( meta.Target.Parameters[0] ).Value;
        }
    }
}
