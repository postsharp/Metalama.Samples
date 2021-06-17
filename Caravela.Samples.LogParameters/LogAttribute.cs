using System;
using System.Text;
using Caravela.Framework.Aspects;
using Caravela.Framework.Code;

public class LogAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        meta.DebugBreak();

        // Build a formatting string.
        var stringBuilder = meta.CompileTime(new StringBuilder());
        stringBuilder.Append(meta.Type.ToDisplayString());
        stringBuilder.Append('.');
        stringBuilder.Append(meta.Method.Name);
        stringBuilder.Append('(');
        int i = meta.CompileTime(0);
        foreach (var p in meta.Parameters)
        {
            var comma = i > 0 ? ", " : "";

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
        var arguments = meta.Parameters.Values.ToArray();
        Console.WriteLine(stringBuilder.ToString() + " started", arguments );

        try
        {
            // Invoke the method.
            dynamic result = meta.Proceed();

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