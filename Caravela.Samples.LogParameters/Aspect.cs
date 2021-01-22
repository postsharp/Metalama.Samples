using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;

public class LogAttribute : OverrideMethodAspect
{
    public override object Template()
    {
        // Build a formatting string and the array of parameters.
        var parameters = new object[target.Method.Parameters.Count + (target.Method.ReturnType.Is(typeof(void)) ? 0 : 1)];
        var stringBuilder = compileTime(new StringBuilder());
        stringBuilder.Append(target.Type.ToDisplayString());
        stringBuilder.Append('.');
        stringBuilder.Append(target.Method.Name);
        stringBuilder.Append('(');
        int i = compileTime(0);
        foreach (var p in target.Parameters)
        {
            string comma = i > 0 ? ", " : "";

            if (p.IsOut)
            {
                stringBuilder.Append($"{comma}{p.Name} = <out> ");
            }
            else
            {
                stringBuilder.Append($"{comma}{p.Name} = {{{i}}}");
                parameters[i] = p.Value;
            }

            i++;
        }
        stringBuilder.Append(')');

        // Write entry message.
        Console.WriteLine(stringBuilder.ToString() + " started", parameters);

        try
        {
            // Invoke the method.
            dynamic result = proceed();

            // Display the success message.
            parameters[i] = result;
            Console.WriteLine(stringBuilder.ToString() + " returned {" + i + "}", parameters);
            return result;
        }
        catch (Exception e)
        {
            // Display the failure message.
            Console.WriteLine(stringBuilder + " failed: " + e, parameters);
            throw;
        }
    }
}