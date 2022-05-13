// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;

internal class OptionalValueTypeAttribute : TypeAspect
{
    private static readonly DiagnosticDefinition<INamedType> _missingNestedTypeError = new(
        "OPT001",
        Severity.Error,
        "The [OptionalValueType] aspect requires '{0}' to contain a nested type named 'Optional'" );

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
        var optionalValuesProperty = builder.Advice.IntroduceProperty( builder.Target, nameof( this.OptionalValues ) );
        optionalValuesProperty.Type = nestedType;
        optionalValuesProperty.InitializerExpression = ExpressionFactory.Parse( $"new {nestedType.Name}()" );

        var optionalValueType = (INamedType) TypeFactory.GetType( typeof( OptionalValue<> ) );

        // For all automatic properties of the target type.
        foreach ( var property in builder.Target.Properties.Where( p => p.IsAutoPropertyOrField ) )
        {
            // Add a property of the same name, but of type OptionalValue<T>, in the nested type.
            var propertyBuilder = builder.Advice.IntroduceProperty( nestedType, nameof( this.OptionalPropertyTemplate ) );
            propertyBuilder.Name = property.Name;
            propertyBuilder.Type = optionalValueType.ConstructGenericInstance( property.Type );

            // Override the property in the target type so that it is forwarded to the nested type.
            builder.Advice.Override(
                property,
                nameof( this.OverridePropertyTemplate ),
                tags: new { optionalProperty = propertyBuilder } );
        }
    }

    [Template]
    public dynamic? OptionalValues { get; private set; }

    [Template]
    public dynamic? OptionalPropertyTemplate { get; set; }

    [Template]
    public dynamic? OverridePropertyTemplate
    {
        get
        {
            var optionalProperty = (IProperty) meta.Tags["optionalProperty"]!;

            return optionalProperty.Invokers.Final.GetValue( meta.This.OptionalValues ).Value;
        }

        set
        {
            var optionalProperty = (IProperty) meta.Tags["optionalProperty"]!;
            var optionalValueBuilder = new ExpressionBuilder();
            optionalValueBuilder.AppendVerbatim( "new " );
            optionalValueBuilder.AppendTypeName( optionalProperty.Type );
            optionalValueBuilder.AppendVerbatim( "( value )" );
            optionalProperty.Invokers.Final.SetValue( meta.This.OptionalValues, optionalValueBuilder.ToValue() );
        }
    }
}

public struct OptionalValue<T>
{
    public bool IsSpecified { get; private set; }

    public T Value { get; }

    public OptionalValue( T value )
    {
        this.Value = value;
        this.IsSpecified = true;
    }

}
