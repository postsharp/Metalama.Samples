using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;

public class LogAttribute : OverrideMethodAspect
{
    public override object Template()
    {
        Console.WriteLine(compileTime(string.Format("Method {0}.{1} started", target.Type.ToString(), target.Method.Name)));

        try
        {
            dynamic result = proceed();

            Console.WriteLine(compileTime(string.Format("Method {0}.{1} succeeded", target.Type.ToString(), target.Method.Name)));
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(compileTime(string.Format("Method {0}.{1} failed: ", target.Type.ToString(), target.Method.Name)) + e.Message);

            throw;
        }
    }
}