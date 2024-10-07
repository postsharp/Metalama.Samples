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
        IProperty OriginatorProperty);

    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        // [<snippet IntroduceType>]
        // Introduce a new private nested class called Memento.
        var mementoType =
            builder.IntroduceClass(
                "Memento",
                buildType: b =>
                    b.Accessibility =
                        Metalama.Framework.Code.Accessibility.Private);
        // [<endsnippet IntroduceType>]

        // [<snippet SelectFields>]
        var originatorFieldsAndProperties = builder.Target.FieldsAndProperties
            .Where(p => p is
            {
                IsStatic: false,
                IsAutoPropertyOrField: true,
                IsImplicitlyDeclared: false,
                Writeability: Writeability.All
            })
            .Where(p =>
                !p.Attributes.OfAttributeType(typeof(MementoIgnoreAttribute))
                    .Any());
        // [<endsnippet SelectFields>]

        // [<snippet IntroduceProperties>]
        // Introduce data properties to the Memento class for each field of the target class.
        var propertyMap = new Dictionary<IFieldOrProperty, IProperty>();

        foreach (var fieldOrProperty in originatorFieldsAndProperties)
        {
            var introducedField = mementoType.IntroduceProperty(
                nameof(this.MementoProperty),
                buildProperty: b =>
                {
                    var trimmedName = fieldOrProperty.Name.TrimStart('_');

                    b.Name = trimmedName.Substring(0, 1).ToUpperInvariant() +
                             trimmedName.Substring(1);
                    b.Type = fieldOrProperty.Type;
                });

            propertyMap.Add(fieldOrProperty, introducedField.Declaration);
        }
        // [<endsnippet IntroduceProperties>]

        // [<snippet IntroduceConstructor>]
        // Add a constructor to the Memento class that records the state of the originator.
        mementoType.IntroduceConstructor(
            nameof(this.MementoConstructorTemplate),
            buildConstructor: b => { b.AddParameter("originator", builder.Target); });
        // [<endsnippet IntroduceConstructor>]

        // [<snippet AddMementoInterface>]
        // Implement the IMemento interface on the Memento class and add its members.   
        mementoType.ImplementInterface(typeof(IMemento),
            whenExists: OverrideStrategy.Ignore);

        var originatorProperty =
            mementoType.IntroduceProperty(nameof(this.Originator));
        // [<endsnippet AddMementoInterface>]

        // [<snippet AddMementoableInterface>]
        // Implement the rest of the IOriginator interface and its members.
        builder.ImplementInterface(typeof(IMementoable));

        builder.IntroduceMethod(
            nameof(this.SaveToMemento),
            whenExists: OverrideStrategy.Override,
            args: new { mementoType = mementoType.Declaration });

        builder.IntroduceMethod(
            nameof(this.RestoreMemento),
            whenExists: OverrideStrategy.Override);
        // [<endsnippet AddMementoableInterface>]

        // Pass the state to the templates.
        // [<snippet SetTag>]
        builder.Tags = new BuildAspectInfo(mementoType.Declaration, propertyMap,
            originatorProperty.Declaration);
        // [<endsnippet SetTag>]
    }

    [Template] public object? MementoProperty { get; }

    [Template] public IMementoable? Originator { get; }

    [Template]
    public IMemento SaveToMemento()
    {
        // [<snippet GetTag>]
        var buildAspectInfo = (BuildAspectInfo)meta.Tags.Source!;
        // [<endsnippet GetTag>]

        // Invoke the constructor of the Memento class and pass this object as the originator.
        return buildAspectInfo.MementoType.Constructors.Single()
            .Invoke((IExpression)meta.This)!;
    }

    [Template]
    public void RestoreMemento(IMemento memento)
    {
        var buildAspectInfo = (BuildAspectInfo)meta.Tags.Source!;

        var typedMemento = meta.Cast(buildAspectInfo.MementoType, memento);

        // Set fields of this instance to the values stored in the Memento.
        foreach (var pair in buildAspectInfo.PropertyMap)
        {
            pair.Key.Value = pair.Value.With((IExpression)typedMemento).Value;
        }
    }

    // [<snippet ConstructorTemplate>]
    [Template]
    public void MementoConstructorTemplate()
    {
        var buildAspectInfo = (BuildAspectInfo)meta.Tags.Source!;

        // Set the originator property and the data properties of the Memento.
        buildAspectInfo.OriginatorProperty.Value = meta.Target.Parameters[0].Value;

        foreach (var pair in buildAspectInfo.PropertyMap)
        {
            pair.Value.Value = pair.Key.With(meta.Target.Parameters[0]).Value;
        }
    }
    // [<endsnippet ConstructorTemplate>]
}