﻿using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

/// <summary>
/// Validates that the builder interface contains all properties of the built interface and all are writeable.
/// </summary>
[Inheritable]
internal class ValidateBuilderInterfaceAttribute : TypeAspect
{
    private static DiagnosticDefinition<INamedType> InvalidType =
        new DiagnosticDefinition<INamedType>( "MY001", Severity.Error, "The aspect is used on {0}. Only interfaces extending IBuilder<T> are supported." );

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        if (builder.Target.Origin is { Kind: DeclarationOriginKind.Aspect} )
        {
            // We do not validate builder types generated by our aspect.
            return;
        }

        var directlyImplementedBuilders =
            builder.Target.ImplementedInterfaces
            .SelectMany( i => i.AllImplementedInterfaces )
            .Distinct()
            .Where( i => i.Definition == TypeFactory.GetType( typeof( IBuilder<> ) ) ).ToList();

        if (builder.Target is not { TypeKind: TypeKind.Interface} && directlyImplementedBuilders is { Count: >0 } )
        {
            builder.Diagnostics.Report( InvalidType.WithArguments( builder.Target ) );
            return;
        }

        var buildersImplementedByBase =
            builder.Target.BaseType != null
            ? builder.Target.BaseType.AllImplementedInterfaces
                .Where( i => i.Definition == TypeFactory.GetType( typeof( IBuilder<> ) ) )
                .Distinct()
            : Array.Empty<INamedType>();

        var buildersToImplement =
            directlyImplementedBuilders.Except( buildersImplementedByBase );

        foreach ( var builderToImplement in buildersToImplement )
        {
        }
    }
}