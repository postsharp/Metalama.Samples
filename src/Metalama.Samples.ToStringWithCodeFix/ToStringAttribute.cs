using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.CodeFixes;

namespace Metalama.Samples.ToString
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [CompileTime] // TODO: should not be necessary to add [CompileTime]
    public  class NotToStringAttribute : Attribute { }

    [LiveTemplate]
    public class ToStringAttribute : TypeAspect
    {
        
        public override void BuildAspect(IAspectBuilder<INamedType> builder)
        {
            base.BuildAspect(builder);

            // For each field, suggest a code fix to remove from ToString.
            foreach ( var field in builder.Target.FieldsAndProperties.Where(f => !f.IsStatic))
            {
                if (!field.Attributes.Any(a => a.Type.Is(typeof(NotToStringAttribute))))
                {
                    builder.Diagnostics.Suggest( CodeFixFactory.AddAttribute( field, typeof(NotToStringAttribute), "Exclude from [ToString]"), field );
                }
            }

            // Suggest to switch to manual implementation.
            if ( builder.AspectInstance.Predecessors[0].Instance is IAttribute attribute  )
            {
                builder.Diagnostics.Suggest(
                    new CodeFix( "Switch to manual implementation", codeFixBuilder => this.ImplementManually( codeFixBuilder, builder.Target ) ),
                    attribute );
            }
        }

        [CompileTime]
        private async Task ImplementManually(ICodeActionBuilder builder, INamedType targetType)
        {
            await builder.ApplyAspectAsync(targetType, this);
            await builder.RemoveAttributesAsync(targetType, typeof(ToStringAttribute));
            await builder.RemoveAttributesAsync(targetType, typeof(NotToStringAttribute));
        }

        [Introduce(WhenExists = OverrideStrategy.Override, Name = "ToString")]
        public string IntroducedToString()
        {
            var stringBuilder = new InterpolatedStringBuilder();
            stringBuilder.AddText("{ ");
            stringBuilder.AddText(meta.Target.Type.Name);
            stringBuilder.AddText(" ");

            var fields = meta.Target.Type.FieldsAndProperties.Where(f => !f.IsStatic).ToList();

            var i = meta.CompileTime(0);

            foreach (var field in fields)
            {
                if ( field.Attributes.Any( a => a.Type.Is(typeof(NotToStringAttribute) ) ) )
                {
                    continue;
                }

                if (i > 0)
                {
                    stringBuilder.AddText(", ");
                }

                stringBuilder.AddText(field.Name);
                stringBuilder.AddText("=");
                stringBuilder.AddExpression(field.Invokers.Final.GetValue(meta.This));

                i++;
            }

            stringBuilder.AddText(" }");


            return stringBuilder.ToValue();
        }
    }
}