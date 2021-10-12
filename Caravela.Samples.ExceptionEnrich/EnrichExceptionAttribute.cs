using System;
using System.Text;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;

public class EnrichExceptionAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        // Compile-time code: create a formatting string containing the method name and placeholder for formatting parameters.
        var methodSignatureBuilder = meta.CompileTime(new StringBuilder());
        methodSignatureBuilder.Append(meta.Target.Type.ToString());
        methodSignatureBuilder.Append('.');
        methodSignatureBuilder.Append(meta.Target.Method.Name);
        methodSignatureBuilder.Append('(');
        var i = meta.CompileTime(0);
        foreach (var p in meta.Target.Parameters)
        {
            var comma = i > 0 ? ", " : "";

            if (p.RefKind == RefKind.Out)
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


        try
        {
            return meta.Proceed();
        }
        catch (Exception e)
        {
            // Get or create a StringBuilder for the exception where we will add additional context data.
            var stringBuilder = (StringBuilder?)e.Data["Context"];
            if (stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
                e.Data["Context"] = stringBuilder;
            }

            // Add current context information to the string builder.
            stringBuilder.AppendFormat("  > " + methodSignatureBuilder.ToString(),
                meta.Target.Parameters.Values.ToArray());
            stringBuilder.AppendLine();

            throw;
        }
    }
}