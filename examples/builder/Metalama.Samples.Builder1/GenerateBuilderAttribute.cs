using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel.DataAnnotations;

namespace Metalama.Samples.Builder1;

public class GenerateBuilderAttribute : TypeAspect
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
        IConstructor SourceConstructor,
        IConstructor BuilderConstructor );

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        base.BuildAspect( builder );

        var sourceType = builder.Target;

        // Create a list of PropertyMapping items for all properties that we want to build using the Builder.
        var properties = sourceType.Properties.Where(
                p => p.Writeability != Writeability.None &&
                     !p.IsStatic )
            .Select(
                p => new PropertyMapping(
                    p,
                    p.Attributes.OfAttributeType( typeof(RequiredAttribute) ).Any() ) )
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
                        IntroductionScope.Instance,
                        buildProperty: p => p.Accessibility = Accessibility.Public )
                    .Declaration;
        }

        // Add a builder constructor accepting the required properties and update the mapping.
        var builderConstructor = builderType.IntroduceConstructor(
            nameof(this.BuilderConstructorTemplate),
            buildConstructor: c =>
            {
                c.Accessibility = Accessibility.Public;

                foreach ( var property in properties.Where( m => m.IsRequired ) )
                {
                    property.BuilderConstructorParameterIndex = c.AddParameter(
                            NameHelper.ToParameterName( property.SourceProperty.Name ),
                            property.SourceProperty.Type )
                        .Index;
                }
            } ).Declaration;

        // Add a Build method to the builder.
        builderType.IntroduceMethod(
            nameof(this.BuildMethodTemplate),
            IntroductionScope.Instance,
            buildMethod: m =>
            {
                m.Name = "Build";
                m.Accessibility = Accessibility.Public;
                m.ReturnType = sourceType;
            } );

        // Add a constructor to the source type with all properties.
        var sourceConstructor = builder.IntroduceConstructor(
                nameof(this.SourceConstructorTemplate),
                buildConstructor: c =>
                {
                    c.Accessibility = Accessibility.Private;

                    foreach ( var property in properties )
                    {
                        property.SourceConstructorParameterIndex = c.AddParameter(
                                NameHelper.ToParameterName( property.SourceProperty.Name ),
                                property.SourceProperty.Type )
                            .Index;
                    }
                } )
            .Declaration;

        // Add a ToBuilder method to the source type.
        builder.IntroduceMethod( nameof(this.ToBuilderMethodTemplate), buildMethod: m =>
        {
            m.Accessibility = Accessibility.Public;
            m.Name = "ToBuilder";
            m.ReturnType = builderType.Declaration;
        } );

        builder.Tags = new Tags( properties, sourceConstructor, builderConstructor );
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

    [Template]
    private dynamic ToBuilderMethodTemplate()
    {
        var tags = (Tags) meta.Tags.Source!;

        // Generate the list of constructor arguments, which much include all required properties.
        var builderConstructorParameters =
            new IExpression[tags.BuilderConstructor.Parameters.Count];

        foreach ( var property in tags.Properties )
        {
            if ( property.IsRequired )
            {
                builderConstructorParameters[property.BuilderConstructorParameterIndex!.Value] =
                    property.SourceProperty;
            }
        }

        // Invoke the constructor and store the result in a local variable.
        var builder =
            tags.BuilderConstructor.CreateInvokeExpression( builderConstructorParameters ).Value!;


        // Set non-required properties.
        foreach ( var property in tags.Properties )
        {
            if ( !property.IsRequired )
            {
                property.BuilderProperty!.With( (IExpression) builder ).Value =
                    property.SourceProperty.Value;
            }
        }

        return builder;
    }
}