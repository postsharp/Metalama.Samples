using System;
using System.Text;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;
using static Caravela.Framework.Aspects.TemplateContext;

public class EnrichExceptionAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        try
        {
            return proceed();
        }
        catch (Exception e)
        {
           
            // Compile-time code: create a formatting string containing the method name and placeholder for formatting parameters.
            var methodSignatureBuilder = compileTime(new StringBuilder());
            methodSignatureBuilder.Append(target.Type.ToString());
            methodSignatureBuilder.Append('.');
            methodSignatureBuilder.Append(target.Method.Name);
            methodSignatureBuilder.Append('(');
            int i = compileTime(0);
            foreach (var p in target.Parameters)
            {
                string comma = i > 0 ? ", " : "";

                if (p.IsOut())
                {
                    methodSignatureBuilder.Append($"{comma}{p.Name} = <out> ");
                }
                else
                {
                    methodSignatureBuilder.Append($"{comma}{{{i}}}");
                }

                i++;
            }
            methodSignatureBuilder.Append(')');

            // Get or create a StringBuilder for the exception where we will add additional context data.
            var stringBuilder = (StringBuilder)e.Data["Context"];
            if (stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
                e.Data["Context"] = stringBuilder;
            }

            // Add current context information to the string builder.
            stringBuilder.AppendFormat("  > " + methodSignatureBuilder.ToString(), target.Parameters.Values.ToArray());
            stringBuilder.AppendLine();

            throw;
        }
    }
}