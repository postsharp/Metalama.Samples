using System;
using System.Text;
using Caravela.Framework.Aspects;

public class LogAttribute : OverrideMethodAspect
{
    public override dynamic OverrideMethod()
    {
        Console.WriteLine(meta.Method.ToDisplayString() + " started.");

        try
        {
            dynamic result = meta.Proceed();

            Console.WriteLine(meta.Method.ToDisplayString() + " succeeded.");
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(meta.Method.ToDisplayString() + " failed: " + e.Message);

            throw;
        }
    }
}