﻿using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.CodeFixes;

[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
public class NotToStringAttribute : Attribute
{
}

[EditorExperience( SuggestAsLiveTemplate = true )]
public class ToStringAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        base.BuildAspect( builder );

        // For each field, suggest a code fix to remove from ToString.
        foreach ( var field in builder.Target.FieldsAndProperties.Where( f => !f.IsStatic && !f.IsImplicitlyDeclared ) )
        {
            if ( !field.Attributes.Any( a => a.Type.Is( typeof(NotToStringAttribute) ) ) )
            {
                builder.Diagnostics.Suggest(
                    CodeFixFactory.AddAttribute( field, typeof(NotToStringAttribute), "Exclude from [ToString]" ),
                    field );
            }
        }

        // Suggest to switch to manual implementation.
        if ( builder.AspectInstance.Predecessors[0].Instance is IAttribute attribute )
        {
            builder.Diagnostics.Suggest(
                new CodeFix( "Switch to manual implementation",
                    codeFixBuilder => this.ImplementManually( codeFixBuilder, builder.Target ) ),
                attribute );
        }
    }

    [CompileTime]
    private async Task ImplementManually( ICodeActionBuilder builder, INamedType targetType )
    {
        await builder.ApplyAspectAsync( targetType, this );
        await builder.RemoveAttributesAsync( targetType, typeof(ToStringAttribute) );
        await builder.RemoveAttributesAsync( targetType, typeof(NotToStringAttribute) );
    }

    [Introduce( WhenExists = OverrideStrategy.Override, Name = "ToString" )]
    public string IntroducedToString()
    {
        var stringBuilder = new InterpolatedStringBuilder();
        stringBuilder.AddText( "{ " );
        stringBuilder.AddText( meta.Target.Type.Name );
        stringBuilder.AddText( " " );

        var fields = meta.Target.Type.FieldsAndProperties.Where( f => !f.IsStatic && !f.IsImplicitlyDeclared ).ToList();

        var i = meta.CompileTime( 0 );

        foreach ( var field in fields )
        {
            if ( field.Attributes.Any( a => a.Type.Is( typeof(NotToStringAttribute) ) ) )
            {
                continue;
            }

            if ( i > 0 )
            {
                stringBuilder.AddText( ", " );
            }

            stringBuilder.AddText( field.Name );
            stringBuilder.AddText( "=" );
            stringBuilder.AddExpression( field.Value );

            i++;
        }

        stringBuilder.AddText( " }" );


        return stringBuilder.ToValue();
    }
}