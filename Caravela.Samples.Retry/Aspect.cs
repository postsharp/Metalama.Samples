using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;

public class RetryAttribute : OverrideMethodAspect
{
    public int Attempts { get; set; } = 5;

    public override dynamic OverrideMethod()
    {
        for (int i = 0; ; i++)
        {
            try
            {
                return proceed();
            }
            catch (Exception e) when (i < Attempts)
            {
                Console.WriteLine(e.Message + " Retrying.");
            }
        }
    }
}