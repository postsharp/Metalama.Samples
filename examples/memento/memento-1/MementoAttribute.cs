using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

public sealed class MementoAttribute : TypeAspect
{
    [CompileTime]
    private record BuildAspectInfo(
        // The newly introduced Memento type.
        INamedType MementoType, 
        // Mapping from fields or properties in the Originator to the corresponding property
        // in the Memento type.
        Dictionary<IFieldOrProperty, IProperty> PropertyMap,
        // The Originator property in the new Memento type.
        IProperty OriginatorProperty );

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        /*<IntroduceType>*/
        // Introduce a new private nested class called Memento.
        var mementoType =  
            builder.IntroduceClass(
                "Memento",
                buildType: b => b.Accessibility = Metalama.Framework.Code.Accessibility.Private ); /*</IntroduceType>*/ 
        
        var originatorFieldsAndProperties = builder.Target.FieldsAndProperties /*<SelectFields>*/
            .Where( p => p is
            {
                IsStatic: false,
                IsAutoPropertyOrField: true, 
                IsImplicitlyDeclared: false,
                Writeability: Writeability.All
            } )
            .Where( p => !p.Attributes.OfAttributeType( typeof(MementoIgnoreAttribute) ).Any() ); /*</SelectFields>*/

        // Introduce data properties to the Memento class for each field of the target class.
        var propertyMap = new Dictionary<IFieldOrProperty, IProperty>();  /*<IntroduceProperties>*/

        foreach ( var fieldOrProperty in originatorFieldsAndProperties )
        {
            var introducedField = mementoType.IntroduceProperty(
                nameof(this.MementoProperty),
                buildProperty: b =>
                {
                    var trimmedName = fieldOrProperty.Name.TrimStart( '_' );

                    b.Name = trimmedName.Substring( 0, 1 ).ToUpperInvariant() +
                             trimmedName.Substring( 1 );
                    b.Type = fieldOrProperty.Type;
                } );

            propertyMap.Add( fieldOrProperty, introducedField.Declaration );
        } /*</IntroduceProperties>*/
        
        // Add a constructor to the Memento class that records the state of the originator.
        mementoType.IntroduceConstructor( /*<IntroduceConstructor>*/
            nameof(this.MementoConstructorTemplate),
            buildConstructor: b => { b.AddParameter( "originator", builder.Target ); } ); /*</IntroduceConstructor>*/


        // Implement the ISnapshot interface on the Memento class and add its members.   
        mementoType.ImplementInterface( typeof(IMemento), whenExists: OverrideStrategy.Ignore ); /*<AddMementoInterface>*/
        
        var originatorProperty =
            mementoType.IntroduceProperty( nameof(this.Originator) );  /*</AddMementoInterface>*/
        
        // Implement the rest of the IOriginator interface and its members.
        builder.ImplementInterface( typeof(IMementoable) ); /*<AddMementoableInterface>*/

        builder.IntroduceMethod(
            nameof(this.SaveToMemento),
            whenExists: OverrideStrategy.Override,
            args: new { mementoType = mementoType.Declaration } );

        builder.IntroduceMethod(
            nameof(this.RestoreMemento),
            whenExists: OverrideStrategy.Override ); /*</AddMementoableInterface>*/

        // Pass the state to the templates.
        builder.Tags = new BuildAspectInfo( mementoType.Declaration, propertyMap, /*<SetTag>*/
            originatorProperty.Declaration ); /*</SetTag>*/
    }

    [Template] public object? MementoProperty { get; } 

    [Template] public IMementoable? Originator { get; }

    [Template]
    public IMemento SaveToMemento()
    {
        var buildAspectInfo = (BuildAspectInfo) meta.Tags.Source!; /*<GetTag>*/
        /*</GetTag>*/

        // Invoke the constructor of the Memento class and pass this object as the originator.
        return buildAspectInfo.MementoType.Constructors.Single()
            .Invoke( (IExpression) meta.This )!;
    }

    [Template]
    public void RestoreMemento( IMemento memento )
    {
        var buildAspectInfo = (BuildAspectInfo) meta.Tags.Source!;

        var typedSnapshot = meta.Cast( buildAspectInfo.MementoType, memento );

        // Set fields of this instance to the values stored in the Snapshot.
        foreach ( var pair in buildAspectInfo.PropertyMap )
        {
            pair.Key.Value = pair.Value.With( (IExpression) typedSnapshot ).Value;
        }
    }

    [Template] /*<ConstructorTemplate>*/
    public void MementoConstructorTemplate()
    {
        var buildAspectInfo = (BuildAspectInfo) meta.Tags.Source!;

        // Set the originator property and the data properties of the Snapshot.
        buildAspectInfo.OriginatorProperty.Value = meta.Target.Parameters[0].Value;

        foreach ( var pair in buildAspectInfo.PropertyMap )
        {
            pair.Value.Value = pair.Key.With( meta.Target.Parameters[0] ).Value;
        }
    } /*</ConstructorTemplate>*/
}