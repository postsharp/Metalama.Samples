using System;
using System.Text;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;
using static Caravela.Framework.Aspects.TemplateContext;

public class LogAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        // Build a formatting string and the array of parameters.
        var stringBuilder = compileTime(new StringBuilder());
        stringBuilder.Append(target.Type.ToDisplayString());
        stringBuilder.Append('.');
        stringBuilder.Append(target.Method.Name);
        stringBuilder.Append('(');
        int i = compileTime(0);
        foreach (var p in target.Parameters)
        {
            string comma = i > 0 ? ", " : "";

            if (p.IsOut())
            {
                stringBuilder.Append($"{comma}{p.Name} = <out> ");
            }
            else
            {
                stringBuilder.Append($"{comma}{p.Name} = {{{i}}}");
            }

            i++;
        }
        stringBuilder.Append(')');

        // Write entry message.
        var arguments = target.Parameters.Values.ToArray();
        Console.WriteLine(stringBuilder.ToString() + " started", arguments );

        try
        {
            // Invoke the method.
            dynamic result = proceed();

            // Display the success message.
            Console.WriteLine( string.Format( stringBuilder.ToString(), arguments) + " returned " + result );
            return result;
        }
        catch (Exception e)
        {
            // Display the failure message.
            Console.WriteLine(stringBuilder + " failed: " + e, arguments);
            throw;
        }
    }
}