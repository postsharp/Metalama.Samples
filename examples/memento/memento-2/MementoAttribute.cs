using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;

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
        var mementoPropertyList = new List<IProperty>();
        var originatorFieldOrPropertyList = new List<IFieldOrProperty>();

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

            // Introduce property to the memento class that stores the value.
            var introducedProperty = mementoType.IntroduceProperty(
                nameof( this.MementoProperty ),
                buildProperty : b =>
                {
                    var trimmedName = fieldOrProperty.Name.TrimStart( '_' );

                    b.Name = trimmedName.Substring( 0, 1 ).ToUpperInvariant() + trimmedName.Substring( 1 );
                    b.Type = fieldOrProperty.Type;
                } );

            mementoPropertyMap.Add( fieldOrProperty, introducedProperty.Declaration );
            mementoPropertyList.Add( introducedProperty.Declaration );
            originatorFieldOrPropertyList.Add( fieldOrProperty );

            // Override the property in the target class to use the transacted value.
            builder.Advice.OverrideAccessors(
                fieldOrProperty,
                nameof( GetTransactedProperty ),
                nameof( SetTransactedProperty ),
                args: new
                {
                    mementoType = mementoType.Declaration,
                    mementoProperty = introducedProperty.Declaration,
                } );
        }

        // Introduce a Commit method that commits the memento.
        mementoType.IntroduceMethod(
            nameof( Commit ),
            args: new { originatorProperty = originatorProperty.Declaration } );

        // Implement the IMemento interface on the Memento class.
        mementoType.ImplementInterface( typeof( ITransactionMemento ) );

        // Add a constructor to the Memento class that records the state of the memento object.
        mementoType.IntroduceConstructor(
            nameof( MementoConstructorTemplate ),
            buildConstructor: b =>
            {
                b.AddParameter( "originator", builder.Target );

                foreach ( var mementoProperty in mementoPropertyList )
                {
                    var parameter = b.AddParameter(
                        mementoProperty.Name.Substring( 0, 1 ).ToLowerInvariant() + mementoProperty.Name.Substring( 1 ),
                        mementoProperty.Type );
                }
            },
            args: new { originatorProperty = originatorProperty.Declaration, propertyList = mementoPropertyList } );

        // Introduce a Capture method to the target class, that creates a Memento object.
        builder.IntroduceMethod(
            nameof( Capture ),
            args: new { mementoType = mementoType.Declaration, fieldOrPropertyList = originatorFieldOrPropertyList } );

        // Introduce a Restore method to the target class, that loads the state of the object from a Memento.
        builder.IntroduceMethod(
            nameof( Restore ),
            args: new { mementoType = mementoType.Declaration, propertyMap = mementoPropertyMap } );

        // Implement the rest of the IOriginator interface.
        builder.ImplementInterface( typeof( IOriginator ) );
    }

    [IntroduceDependency]
    public ICaretaker? Caretaker { get; }

    [Template]
    public object? MementoProperty { get; set; }

    [Template]
    public object? Originator { get; }

    [Template]
    public dynamic? GetTransactedProperty( [CompileTime] INamedType mementoType, [CompileTime] IProperty mementoProperty )
    {
        // Ask the caretaker for the current transaction.
        var transaction = this.Caretaker?.CurrentTransaction;
        if ( transaction != null )
        {
            // Use the value from the current transaction.
            var editableMemento = transaction.GetTransactionMemento( meta.This );
            return mementoProperty.With( (IExpression) meta.Cast( mementoType, editableMemento ) ).Value;
        }
        else
        {
            // Use the value from the committed state.
            return meta.Proceed();
        }
    }


    [Template]
    public void SetTransactedProperty( dynamic? value, [CompileTime] INamedType mementoType, [CompileTime] IProperty mementoProperty )
    {
        // Ask the caretaker for the current transaction.
        var transaction = this.Caretaker?.CurrentTransaction;
        if ( transaction != null )
        {
            // Set the value from the current transaction.
            var editableMemento = transaction.GetTransactionMemento( meta.This );
            mementoProperty.With( (IExpression) meta.Cast( mementoType, editableMemento) ).Value = value;
        }
        else
        {
            // Set the value to the committed state.
            meta.Proceed();
        }
    }

    [Template]
    public IMemento Capture( [CompileTime] INamedType mementoType, [CompileTime] List<IFieldOrProperty> fieldOrPropertyList )
    {
        List<IExpression> arguments = [(IExpression) meta.This];

        foreach ( var fieldOrProperty in fieldOrPropertyList )
        {
            arguments.Add( fieldOrProperty.With( InvokerOptions.Base ) );
        }

        // Invoke the constructor of the Memento class and pass this object as the originator.
        return mementoType.Constructors.Single().Invoke( arguments )!;
    }

    [Template]
    public void Restore( IMemento memento, [CompileTime] INamedType mementoType, [CompileTime] Dictionary<IFieldOrProperty, IProperty> propertyMap)
    {
        var onPropertyChangedMethod = meta.Target.Type.AllMethods.OfName( "OnPropertyChanged" ).Single( x => x.Parameters is [{ Type.SpecialType: SpecialType.String }] );

        // Set fields of this instance to the values stored in the Memento.
        foreach ( var pair in propertyMap )
        {
            var old = pair.Key.Value;
            pair.Key.With(InvokerOptions.Base).Value = pair.Value.With( (IExpression) meta.Cast( mementoType, memento )! ).Value;
            onPropertyChangedMethod.Invoke( pair.Value.Name );
        }
    }

    [Template]
    public void Commit( [CompileTime] IProperty originatorProperty )
    {
        originatorProperty.Value!.Restore( meta.This );
    }

    [Template]
    public void MementoConstructorTemplate( [CompileTime] IProperty originatorProperty, [CompileTime] List<IProperty> propertyList)
    {
        // Set the originator property and the data properties of the Memento.
        originatorProperty.Value = meta.Target.Parameters[0];

        var index = meta.CompileTime( 0 );

        foreach ( var parameter in meta.Target.Parameters.Skip( 1 ) )
        {
            propertyList[index].Value = parameter.Value;

            index++;
        }
    }
}
