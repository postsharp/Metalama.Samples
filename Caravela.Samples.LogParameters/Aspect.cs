using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;

public class LogAttribute : OverrideMethodAspect
{
    public override object Template()
    {
        var parameters = new object[target.Method.Parameters.Count + (target.Method.ReturnType.Is(typeof(void)) ? 0 : 1)];
        var stringBuilder = compileTime(new StringBuilder());
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

        Console.WriteLine(stringBuilder.ToString() + " started", parameters);

        try
        {
            dynamic result = proceed();
            parameters[i] = result;
            Console.WriteLine(stringBuilder.ToString() + " returned {" + i + "}", parameters);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(stringBuilder + " failed: " + e, parameters);
            throw;
        }
    }
}