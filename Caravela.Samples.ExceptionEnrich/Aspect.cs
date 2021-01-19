using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;

public class EnrichExceptionAttribute : OverrideMethodAspect
{
    public override object Template()
    {
        try
        {
            return proceed();
        }
        catch (Exception e)
        {
            // Get or create a StringBuilder for the exception where we will add additional context data.
            var stringBuilder = (StringBuilder)e.Data["Context"];
            if (stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
                e.Data["Context"] = stringBuilder;
            }

            // Add current context information to the string builder.

            stringBuilder.Append(target.Type.ToString());
            stringBuilder.Append('.');
            stringBuilder.Append(target.Method.Name);
            stringBuilder.Append('(');
            int i = compileTime(0);
            foreach (var p in target.Parameters)
            {
                string comma = i > 0 ? ", " : "";

                if (p.IsOut)
                {
                    stringBuilder.Append("<out>");
                }
                else
                {
                    stringBuilder.Append(p.Value);
                }

                i++;
            }
            stringBuilder.Append(')');
            stringBuilder.AppendLine();

            throw;
        }
    }
}