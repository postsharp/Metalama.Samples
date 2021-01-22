using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;

public class LogAttribute : OverrideMethodAspect
{
    public override object Template()
    {
        Console.WriteLine(target.Method.ToDisplayString() + " started.");

        try
        {
            dynamic result = proceed();

            Console.WriteLine(target.Method.ToDisplayString() + " succeeded.");
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(target.Method.ToDisplayString() + " failed: " + e.Message);

            throw;
        }
    }
}