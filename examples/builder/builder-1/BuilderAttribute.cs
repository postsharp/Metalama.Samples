using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel.DataAnnotations;

public class BuilderAttribute : TypeAspect
{
    [CompileTime]
    private class PropertyMapping
    {
        public PropertyMapping( IProperty sourceProperty, bool isRequired )
        {
            this.SourceProperty = sourceProperty;
            this.IsRequired = isRequired;
        }

        public IProperty SourceProperty { get; }

        public bool IsRequired { get; }

        public IProperty? BuilderProperty { get; set; }

        public int? SourceConstructorParameterIndex { get; set; }

        public int? BuilderConstructorParameterIndex { get; set; }
    }

    [CompileTime]
    private record Tags(
        IReadOnlyList<PropertyMapping> Properties,
        IConstructor SourceConstructor );

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        base.BuildAspect( builder );

        // Create a list of PropertyMapping items for all properties that we want to build using the Builder.
        var properties = builder.Target.Properties.Where(
                p => p.Writeability != Writeability.None &&
                     !p.IsStatic )
            .Select(
                p => new PropertyMapping(
                    p,
                    p.Attributes.OfAttributeType( typeof( RequiredAttribute ) ).Any() ) )
            .ToList();

        // Introduce the Builder nested type.
        var builderType = builder.IntroduceClass(
            "Builder",
            buildType: t => t.Accessibility = Accessibility.Public );

        // Add builder properties and update the mapping.
        foreach ( var property in properties )
        {
            property.BuilderProperty =
                builderType.IntroduceAutomaticProperty(
                        property.SourceProperty.Name,
                        property.SourceProperty.Type,
                        IntroductionScope.Instance )
                    .Declaration;
        }

        // Add a builder constructor accepting the required properties and update the mapping.
        if ( properties.Any( m => m.IsRequired ) )
        {
            builderType.IntroduceConstructor(
                nameof( this.BuilderConstructorTemplate ),
                buildConstructor: c =>
                {
                    foreach ( var property in properties.Where( m => m.IsRequired ) )
                    {
                        property.BuilderConstructorParameterIndex = c.AddParameter(
                                property.SourceProperty.Name,
                                property.SourceProperty.Type )
                            .Index;
                    }
                } );
        }

        // Add a Build method to the builder.
        builderType.IntroduceMethod(
            nameof( this.BuildMethodTemplate ),
            IntroductionScope.Instance,
            buildMethod: m =>
            {
                m.Name = "Build";
                m.Accessibility = Accessibility.Public;
                m.ReturnType = builder.Target;

                foreach ( var property in properties )
                {
                    property.BuilderConstructorParameterIndex =
                        m.AddParameter( property.SourceProperty.Name, property.SourceProperty.Type )
                            .Index;
                }
            } );

        // Add a constructor to the source type with all properties.
        var constructor = builder.IntroduceConstructor(
                nameof( this.SourceConstructorTemplate ),
                buildConstructor: c =>
                {
                    c.Accessibility = Accessibility.Private;

                    foreach ( var property in properties )
                    {
                        property.SourceConstructorParameterIndex = c.AddParameter(
                                property.SourceProperty.Name,
                                property.SourceProperty.Type )
                            .Index;
                    }
                } )
            .Declaration;

        builder.Tags = new Tags( properties, constructor );
    }

    [Template]
    private void BuilderConstructorTemplate()
    {
        var tags = (Tags) meta.Tags.Source!;

        foreach ( var property in tags.Properties.Where( p => p.IsRequired ) )
        {
            property.BuilderProperty!.Value =
                meta.Target.Parameters[property.BuilderConstructorParameterIndex!.Value].Value;
        }
    }

    [Template]
    private void SourceConstructorTemplate()
    {
        var tags = (Tags) meta.Tags.Source!;

        foreach ( var property in tags.Properties )
        {
            property.BuilderProperty!.Value =
                meta.Target.Parameters[property.SourceConstructorParameterIndex!.Value].Value;
        }
    }

    [Template]
    private dynamic BuildMethodTemplate()
    {
        var tags = (Tags) meta.Tags.Source!;

        return tags.SourceConstructor.Invoke( tags.Properties.Select( x => x.BuilderProperty! ) )!;
    }
}