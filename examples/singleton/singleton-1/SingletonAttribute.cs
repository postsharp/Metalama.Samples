using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;

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
        builder.Advice.IntroduceProperty(
            builder.Target,
            nameof(Instance),
            buildProperty: propertyBuilder => /*<IntroduceInstanceProperty>*/
            {
                propertyBuilder.Type = builder.Target;

                var initializer = new ExpressionBuilder();
                initializer.AppendVerbatim( "new " );
                initializer.AppendTypeName( builder.Target );
                initializer.AppendVerbatim( "()" );

                propertyBuilder.InitializerExpression = initializer.ToExpression();
            } ); /*</IntroduceInstanceProperty>*/

        foreach ( var constructor in builder.Target.Constructors ) /*<PrivateConstructorReport>*/
        {
            if ( constructor.Accessibility != Accessibility.Private )
            {
                builder.Diagnostics.Report( 
                    _constructorHasToBePrivate.WithArguments( (constructor, builder.Target) ),
                    location: constructor );
            }
        } /*</PrivateConstructorReport>*/
    }
}