using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Eligibility;

#pragma warning disable CS8618

public class SingletonAttribute : TypeAspect
{
    [Template] /*<InstanceTemplate>*/
    public static object Instance { get; } /*</InstanceTemplate>*/

    private static readonly DiagnosticDefinition<(IConstructor, INamedType)>
        _constructorHasToBePrivate /*<PrivateConstructorDiagnostic>*/
            = new(
                "SING01",
                Severity.Warning,
                "The '{0}' constructor must be private because the class is [Singleton]."); /*</PrivateConstructorDiagnostic>*/

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Introduce the property.
        builder.Advice.IntroduceProperty( /*<IntroduceInstanceProperty>*/
            builder.Target,
            nameof(Instance),
            buildProperty: propertyBuilder =>
            {
                propertyBuilder.Type = builder.Target;

                var initializer = new ExpressionBuilder();
                initializer.AppendVerbatim( "new " );
                initializer.AppendTypeName( builder.Target );
                initializer.AppendVerbatim( "()" );

                propertyBuilder.InitializerExpression = initializer.ToExpression();
            } ); /*</IntroduceInstanceProperty>*/

        // Verify constructors.
        foreach ( var constructor in builder.Target.Constructors ) /*<PrivateConstructorReport>*/
        {
            if ( constructor.Accessibility != Accessibility.Private && !constructor.IsImplicitlyDeclared )
            {
                builder.Diagnostics.Report(
                    _constructorHasToBePrivate.WithArguments( (constructor, builder.Target) ),
                    constructor );
            }
        } /*</PrivateConstructorReport>*/

        // If there is no explicit constructor, add one.
        if ( builder.Target.Constructors.All( c => c.IsImplicitlyDeclared ) ) /*<AddPrivateConstructor>*/
        {
            builder.IntroduceConstructor( nameof(this.ConstructorTemplate),
                buildConstructor: c => c.Accessibility = Accessibility.Private );
        } /*</AddPrivateConstructor>*/
    }

    [Template]
    private void ConstructorTemplate() { }

    public override void BuildEligibility( IEligibilityBuilder<INamedType> builder ) =>
        builder.MustSatisfy( t => t.TypeKind is TypeKind.Class,
            t => $"{t} must be a class" );
}