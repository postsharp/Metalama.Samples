﻿using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;


[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)] /*<AttributeUsage>*/
/*</AttributeUsage>*/
public class GenerateEnumViewModelAttribute : CompilationAspect /*<ClassHeader>*/
{
    public Type EnumType { get; }
    public string TargetNamespace { get; }

    public GenerateEnumViewModelAttribute(Type enumType, string targetNamespace)
    {
        this.EnumType = enumType;
        this.TargetNamespace = targetNamespace;
    }

    /*</ClassHeader>*/

    public override void BuildAspect(IAspectBuilder<ICompilation> builder) /*<MultiInstance>*/
    {
        ImplementViewModel(this);

        foreach (var secondaryInstance in builder.AspectInstance.SecondaryInstances)
        {
            ImplementViewModel((GenerateEnumViewModelAttribute)secondaryInstance.Aspect);
        }

        void ImplementViewModel(
            GenerateEnumViewModelAttribute aspectInstance) /*</MultiInstance>*/
        {
            var enumType =
                (INamedType)TypeFactory.GetType(aspectInstance.EnumType); /*<ValidateInputs>*/

            if (enumType.TypeKind != TypeKind.Enum)
            {
                builder.Diagnostics.Report(
                    DiagnosticDefinitions.NotAnEnumError.WithArguments(enumType));
                builder.SkipAspect();
                return;
            } /*</ValidateInputs>*/

            /*<IntroduceClass>*/
            // Introduce the ViewModel type.
            var viewModelType = builder
                .WithNamespace(this.TargetNamespace)
                .IntroduceClass(
                    enumType.Name + "ViewModel",
                    buildType:
                    type =>
                    {
                        type.Accessibility = enumType.Accessibility;
                        type.IsSealed = true;
                    });

            // Introduce the _value field.
            viewModelType.IntroduceField("_value", enumType,
                IntroductionScope.Instance,
                buildField: field => { field.Writeability = Writeability.ConstructorOnly; });

            // Introduce the constructor.
            viewModelType.IntroduceConstructor(
                nameof(this.ConstructorTemplate),
                args: new { T = enumType });
            /*</IntroduceClass>*/


            /*<AddProperties>*/
            // Get the field type and decides the template.
            var isFlags = enumType.Attributes.Any(a => a.Type.Is(typeof(FlagsAttribute)));
            var template = isFlags ? nameof(this.IsFlagTemplate) : nameof(this.IsMemberTemplate);

            // Introduce a property into the view-model type for each enum member.
            foreach (var member in enumType.Fields)
            {
                viewModelType.IntroduceProperty(
                    template,
                    tags: new { member },
                    buildProperty: p => p.Name = "Is" + member.Name);
            }
            /*</AddProperties>*/
        }
    }

    // Template for the non-flags enum member.
    [Template]
    public bool IsMemberTemplate => meta.This._value == ((IField)meta.Tags["member"]!).Value;

    // Template for a flag enum member.
    [Template]
    public bool IsFlagTemplate
    {
        get
        {
            var field = (IField)meta.Tags["member"]!;

            // Note that the next line does not work for the "zero" flag, but currently Metalama
            // does not expose the constant value of the enum member so we cannot test its
            // value at compile time.
            return (meta.This._value & field.Value) == ((IField)meta.Tags["member"]!).Value;
        }
    }

    [Template] /*<ConstructorTemplate>*/
    public void ConstructorTemplate<[CompileTime] T>(T value) =>
        meta.This._value = value!; /*</ConstructorTemplate>*/
}