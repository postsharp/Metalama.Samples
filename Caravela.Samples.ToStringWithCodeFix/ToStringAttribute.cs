using System;
using System.Linq;
using System.Threading.Tasks;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;
using Caravela.Framework.Code.SyntaxBuilders;
using Caravela.Framework.CodeFixes;
using Caravela.Framework.Diagnostics;

namespace Caravela.Samples.ToString
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [CompileTime] // TODO: should not be necessary to add [CompileTime]
    public  class NotToStringAttribute : Attribute
    {
    }

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
                    builder.Diagnostics.Suggest( field, CodeFix.AddAttribute( field, typeof(NotToStringAttribute), "Exclude from [ToString]") );
                }
            }

            // Suggest to switch to manual implementation.
            if ( builder.AspectInstance.Predecessors[0].Instance is IAttribute attribute  )
            {
                builder.Diagnostics.Suggest(
                    attribute, 
                    CodeFix.Create( codeFixBuilder => this.ImplementManually(codeFixBuilder, builder.Target), "Switch to manual implementation") );
            }
        }

        private async Task ImplementManually(ICodeFixBuilder builder, INamedType targetType)
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