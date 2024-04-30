﻿using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;

public class SingletonAttribute : TypeAspect
{
    /*<InstanceTemplate>*/[Template]
    public static object Instance { get; }/*</InstanceTemplate>*/

    /*<PrivateConstructorDiagnostic>*/private static readonly DiagnosticDefinition<(IConstructor, INamedType)> _constructorHasToBePrivate
        = new( "SING01", Severity.Warning, "The constructor {0} of the singleton class {1} has to be private." );/*</PrivateConstructorDiagnostic>*/

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        /*<IntroduceInstanceProperty>*/builder.Advice.IntroduceProperty( builder.Target, nameof(Instance), buildProperty: propertyBuilder =>
        {
            propertyBuilder.Type = builder.Target;

            var initializer = new ExpressionBuilder();
            initializer.AppendVerbatim( "new " );
            initializer.AppendTypeName( builder.Target );
            initializer.AppendVerbatim( "()" );

            propertyBuilder.InitializerExpression = initializer.ToExpression();
        } );/*</IntroduceInstanceProperty>*/

        /*<PrivateConstructorReport>*/foreach ( var constructor in builder.Target.Constructors )
        {
            if ( constructor.Accessibility != Accessibility.Private )
            {
                builder.Diagnostics.Report( _constructorHasToBePrivate.WithArguments( (constructor, builder.Target) ), location: constructor );
            }
        }/*</PrivateConstructorReport>*/
    }
}