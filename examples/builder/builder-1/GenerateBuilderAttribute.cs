using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace Metalama.Samples.Builder1;

public partial class GenerateBuilderAttribute : TypeAspect
{
    /*<InitializeMapping>*/
    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        base.BuildAspect(builder);

        var sourceType = builder.Target;

        // Create a list of PropertyMapping items for all properties that we want to build using the Builder.
        var properties = sourceType.Properties.Where(
                p => p.Writeability != Writeability.None &&
                     !p.IsStatic)
            .Select(
                p => new PropertyMapping(p,
                    p.Attributes.OfAttributeType(typeof(RequiredAttribute)).Any()))
            .ToList(); /*</InitializeMapping>*/

        /*<IntroduceBuilder>*/
        // Introduce the Builder nested type.
        var builderType = builder.IntroduceClass(
            "Builder",
            buildType: t => t.Accessibility = Accessibility.Public); /*</IntroduceBuilder>*/

        /*<IntroduceProperties>*/
        // Add builder properties and update the mapping.
        foreach (var property in properties)
        {
            property.BuilderProperty =
                builderType.IntroduceAutomaticProperty(
                        property.SourceProperty.Name,
                        property.SourceProperty.Type,
                        buildProperty: p =>
                        {
                            p.Accessibility = Accessibility.Public;
                            p.InitializerExpression = property.SourceProperty.InitializerExpression;
                        })
                    .Declaration;
        } /*</IntroduceProperties>*/

        /*<IntroducePublicConstructor>*/
        // Add a builder constructor accepting the required properties and update the mapping.
        builderType.IntroduceConstructor(
            nameof(this.BuilderConstructorTemplate),
            buildConstructor: c =>
            {
                c.Accessibility = Accessibility.Public;

                foreach (var property in properties.Where(m => m.IsRequired))
                {
                    var parameter = c.AddParameter(
                        NameHelper.ToParameterName(property.SourceProperty.Name),
                        property.SourceProperty.Type);

                    property.BuilderConstructorParameterIndex = parameter.Index;
                }
            }); /*</IntroducePublicConstructor>*/

        /*<IntroduceSourceConstructor>*/
        // Add a constructor to the source type with all properties.
        var sourceConstructor = builder.IntroduceConstructor(
                nameof(this.SourceConstructorTemplate),
                buildConstructor: c =>
                {
                    c.Accessibility = Accessibility.Private;

                    foreach (var property in properties)
                    {
                        var parameter = c.AddParameter(
                            NameHelper.ToParameterName(property.SourceProperty.Name),
                            property.SourceProperty.Type);

                        property.SourceConstructorParameterIndex = parameter.Index;
                    }
                })
            .Declaration; /*</IntroduceSourceConstructor>*/

        /*<IntroduceBuildMethod>*/
        // Add a Build method to the builder.
        builderType.IntroduceMethod(
            nameof(this.BuildMethodTemplate),
            IntroductionScope.Instance,
            buildMethod: m =>
            {
                m.Name = "Build";
                m.Accessibility = Accessibility.Public;
                m.ReturnType = sourceType;
            });
        /*</IntroduceBuildMethod>*/

        /*<IntroduceCopyConstructor>*/
        // Add a builder constructor that creates a copy from the source type.
        var builderCopyConstructor = builderType.IntroduceConstructor(
            nameof(this.BuilderCopyConstructorTemplate),
            buildConstructor: c =>
            {
                c.Accessibility = Accessibility.Internal;
                c.Parameters[0].Type = sourceType;
            }).Declaration; /*</IntroduceCopyConstructor>*/

        /*<IntroduceToBuilderMethod>*/
        // Add a ToBuilder method to the source type.
        builder.IntroduceMethod(nameof(this.ToBuilderMethodTemplate), buildMethod: m =>
        {
            m.Accessibility = Accessibility.Public;
            m.Name = "ToBuilder";
            m.ReturnType = builderType.Declaration;
        });
        /*</IntroduceToBuilderMethod>*/

        /*<SetTags>*/
        builder.Tags = new Tags(builder.Target, properties, sourceConstructor,
            builderCopyConstructor); /*</SetTags>*/
    }

    [Template]
    private void BuilderConstructorTemplate()
    {
        var tags = (Tags)meta.Tags.Source!;

        foreach (var property in tags.Properties.Where(p => p.IsRequired))
        {
            property.BuilderProperty!.Value =
                meta.Target.Parameters[property.BuilderConstructorParameterIndex!.Value].Value;
        }
    }

    [Template]
    private void BuilderCopyConstructorTemplate(dynamic source)
    {
        var tags = (Tags)meta.Tags.Source!;

        foreach (var property in tags.Properties)
        {
            property.BuilderProperty!.Value =
                property.SourceProperty.With((IExpression)source).Value;
        }
    }

    [Template]
    private void SourceConstructorTemplate()
    {
        var tags = (Tags)meta.Tags.Source!;

        foreach (var property in tags.Properties)
        {
            property.SourceProperty.Value =
                meta.Target.Parameters[property.SourceConstructorParameterIndex!.Value].Value;
        }
    }

    [Template]
    private dynamic BuildMethodTemplate()
    {
        var tags = (Tags)meta.Tags.Source!;

        // Build the object.
        var instance = tags.SourceConstructor.Invoke(
            tags.Properties.Select(x => x.BuilderProperty!))!;

        // Find and invoke the Validate method, if any.
        var validateMethod = tags.SourceType.AllMethods.OfName("Validate")
            .SingleOrDefault(m => m.Parameters.Count == 0);

        if (validateMethod != null)
        {
            validateMethod.With((IExpression)instance).Invoke();
        }

        // Return the object.
        return instance;
    }

    [Template]
    private dynamic ToBuilderMethodTemplate()
    {
        var tags = (Tags)meta.Tags.Source!;

        return tags.BuilderCopyConstructor.Invoke(meta.This);
    }
}