using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;

internal class OptionalValueTypeAttribute : TypeAspect
{
    private static readonly DiagnosticDefinition<INamedType> _missingNestedTypeError = new(
        "OPT001",
        Severity.Error,
        "The [OptionalValueType] aspect requires '{0}' to contain a nested type named 'Optional'");

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Find the nested type.
        var nestedType = builder.Target.NestedTypes.OfName( "Optional" ).FirstOrDefault();

        if ( nestedType == null )
        {
            builder.Diagnostics.Report( _missingNestedTypeError.WithArguments( builder.Target ), builder.Target );

            return;
        }

        // Introduce a property in the main type to store the Optional object.
        var optionalValuesProperty = builder.Advice.IntroduceProperty( builder.Target, nameof(this.OptionalValues),
            buildProperty: p =>
            {
                p.Type = nestedType;
                p.InitializerExpression = ExpressionFactory.Parse( $"new {nestedType.Name}()" );
            } ).Declaration;


        var optionalValueType = (INamedType) TypeFactory.GetType( typeof(OptionalValue<>) );

        // For all automatic properties of the target type.
        foreach ( var property in builder.Target.Properties.Where( p => p.IsAutoPropertyOrField.GetValueOrDefault() ) )
        {
            // Add a property of the same name, but of type OptionalValue<T>, in the nested type.
            var propertyBuilder = builder.Advice.IntroduceProperty( nestedType, nameof(this.OptionalPropertyTemplate),
                buildProperty: p =>
                {
                    p.Name = property.Name;
                    p.Type = optionalValueType.WithTypeArguments( property.Type );
                } ).Declaration;


            // Override the property in the target type so that it is forwarded to the nested type.
            builder.Advice.Override(
                property,
                nameof(this.OverridePropertyTemplate),
                new { optionalProperty = propertyBuilder } );
        }
    }

    [Template] public dynamic? OptionalValues { get; private set; }

    [Template] public dynamic? OptionalPropertyTemplate { get; set; }

    [Template]
    public dynamic? OverridePropertyTemplate
    {
        get
        {
            var optionalProperty = (IProperty) meta.Tags["optionalProperty"]!;

            return optionalProperty.With( (IExpression) meta.This.OptionalValues ).Value.Value;
        }

        set
        {
            var optionalProperty = (IProperty) meta.Tags["optionalProperty"]!;
            var optionalValueBuilder = new ExpressionBuilder();
            optionalValueBuilder.AppendVerbatim( "new " );
            optionalValueBuilder.AppendTypeName( optionalProperty.Type );
            optionalValueBuilder.AppendVerbatim( "( value )" );
            optionalProperty.With( (IExpression) meta.This.OptionalValues ).Value = optionalValueBuilder.ToValue();
        }
    }
}
