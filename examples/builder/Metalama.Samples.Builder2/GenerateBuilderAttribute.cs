using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Metalama.Samples.Builder2;

[Inheritable]
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

        var hasError = false;

        var sourceType = builder.Target;

        // Find the Builder nested type in the base type.
        INamedType? baseBuilderType = null;
        IConstructor? baseConstructor = null;

        if ( sourceType.BaseType != null && sourceType.BaseType.SpecialType != SpecialType.Object )
        {
            // We need to filter parameters to work around a bug where the Constructors collection
            // contains the implicit constructor.
            var baseTypeConstructors =
                sourceType.BaseType.Constructors.Where( c => c.Parameters.Count > 0 ).ToList();

            if ( baseTypeConstructors.Count != 1 )
            {
                builder.Diagnostics.Report(
                    BuilderDiagnosticDefinitions.BaseTypeMustContainOneConstructor.WithArguments( (
                        sourceType.BaseType, baseTypeConstructors.Count) ) );
                hasError = true;
            }
            else
            {
                baseConstructor = baseTypeConstructors[0];
            }


            var baseBuilderTypes =
                sourceType.BaseType.Definition.Types.OfName( "Builder" ).ToList();

            switch ( baseBuilderTypes.Count )
            {
                case 0:
                    builder.Diagnostics.Report(
                        BuilderDiagnosticDefinitions.BaseTypeMustContainABuilderType.WithArguments(
                            sourceType.BaseType.Definition ) );
                    return;

                case > 1:
                    builder.Diagnostics.Report(
                        BuilderDiagnosticDefinitions.BaseTypeCannotContainMoreThanOneBuilderType
                            .WithArguments( sourceType.BaseType.Definition ) );
                    return;

                default:
                    baseBuilderType = baseBuilderTypes[0];

                    if ( baseBuilderType.Constructors.Count != 1 )
                    {
                        builder.Diagnostics.Report(
                            BuilderDiagnosticDefinitions.BaseTypeMustContainOneConstructor
                                .WithArguments( (baseBuilderType,
                                    baseBuilderType.Constructors.Count) ) );
                        return;
                    }

                    break;
            }
        }

        if ( hasError )
        {
            return;
        }

        // Create a list of PropertyMapping items for all properties that we want to build using the Builder.
        var properties = sourceType.AllProperties.Where(
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
            whenExists: OverrideStrategy.New,
            buildType: t =>
            {
                t.Accessibility = Accessibility.Public;
                t.BaseType = baseBuilderType;
                t.IsSealed = sourceType.IsSealed;
            } );

        // Add builder properties and update the mapping.
        foreach ( var property in properties )
        {
            if ( property.SourceProperty.DeclaringType == sourceType )
            {
                // For properties of the current type, introduce a new property.
                property.BuilderProperty =
                    builderType.IntroduceAutomaticProperty(
                            property.SourceProperty.Name,
                            property.SourceProperty.Type,
                            IntroductionScope.Instance,
                            buildProperty: p => p.Accessibility = Accessibility.Public )
                        .Declaration;
            }
            else if ( baseBuilderType != null )
            {
                // For properties of the base type, find the matching property.
                var baseProperty =
                    baseBuilderType.AllProperties.OfName( property.SourceProperty.Name )
                        .SingleOrDefault();

                if ( baseProperty == null )
                {
                    builder.Diagnostics.Report(
                        BuilderDiagnosticDefinitions.BaseBuilderMustContainProperty.WithArguments( (
                            baseBuilderType, property.SourceProperty.Name) ) );
                    hasError = true;
                }
                else
                {
                    property.BuilderProperty = baseProperty;
                }
            }
        }

        if ( hasError )
        {
            return;
        }

        // Add a builder constructor accepting the required properties and update the mapping.
        var builderConstructor = builderType.IntroduceConstructor(
            nameof(this.BuilderConstructorTemplate),
            buildConstructor: c =>
            {
                c.Accessibility = Accessibility.Public;

                // Adding parameters.
                foreach ( var property in properties.Where( m => m.IsRequired ) )
                {
                    var parameter = c.AddParameter(
                        NameHelper.ToParameterName( property.SourceProperty.Name ),
                        property.SourceProperty.Type );

                    property.BuilderConstructorParameterIndex = parameter.Index;
                }

                // Calling the base constructor.
                if ( baseBuilderType != null )
                {
                    var baseBuilderConstructor = baseBuilderType.Constructors.Single();
                    c.InitializerKind = ConstructorInitializerKind.Base;

                    foreach ( var baseConstructorParameter in baseBuilderConstructor.Parameters )
                    {
                        var thisParameter =
                            c.Parameters.SingleOrDefault( p =>
                                p.Name == baseConstructorParameter.Name );
                        if ( thisParameter != null )
                        {
                            c.AddInitializerArgument( thisParameter );
                        }
                        else
                        {
                            builder.Diagnostics.Report(
                                BuilderDiagnosticDefinitions
                                    .BaseTypeConstructorHasUnexpectedParameter.WithArguments( (
                                        baseBuilderConstructor,
                                        baseConstructorParameter.Name) ) );
                            hasError = true;
                        }
                    }
                }
            } ).Declaration;


        if ( hasError )
        {
            return;
        }

        // Add a Build method to the builder.
        builderType.IntroduceMethod(
            nameof(this.BuildMethodTemplate),
            IntroductionScope.Instance,
            whenExists: OverrideStrategy.New,
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
                    c.Accessibility = sourceType.IsSealed
                        ? Accessibility.Private
                        : Accessibility.Protected;


                    foreach ( var property in properties )
                    {
                        var parameter = c.AddParameter(
                            NameHelper.ToParameterName( property.SourceProperty.Name ),
                            property.SourceProperty.Type );

                        property.SourceConstructorParameterIndex = parameter.Index;
                    }

                    if ( baseConstructor != null )
                    {
                        c.InitializerKind = ConstructorInitializerKind.Base;
                        foreach ( var baseConstructorParameter in baseConstructor.Parameters )
                        {
                            var thisParameter = c.Parameters.SingleOrDefault( p =>
                                p.Name == baseConstructorParameter.Name );
                            if ( thisParameter == null )
                            {
                                builder.Diagnostics.Report(
                                    BuilderDiagnosticDefinitions
                                        .BaseTypeConstructorHasUnexpectedParameter.WithArguments( (
                                            baseConstructor, baseConstructorParameter.Name) ) );
                                hasError = true;
                            }
                            else
                            {
                                c.AddInitializerArgument( thisParameter );
                            }
                        }
                    }
                } )
            .Declaration;

        // Add a ToBuilder method to the source type.
        builder.IntroduceMethod( nameof(this.ToBuilderMethodTemplate),
            whenExists: OverrideStrategy.Override,
            buildMethod: m =>
            {
                m.Accessibility = Accessibility.Public;
                m.Name = "ToBuilder";
                m.ReturnType = builderType.Declaration;
                m.IsVirtual = !sourceType.IsSealed;
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
            if ( property.SourceProperty.DeclaringType == meta.Target.Type )
            {
                property.BuilderProperty!.Value =
                    meta.Target.Parameters[property.SourceConstructorParameterIndex!.Value].Value;
            }
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