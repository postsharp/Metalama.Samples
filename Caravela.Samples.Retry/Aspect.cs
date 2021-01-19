using System;
using System.Text;
using Caravela.Framework.Aspects;
using static Caravela.Framework.Aspects.TemplateContext;

public class RetryAttribute : OverrideMethodAspect
{
    public override object Template()
    {
        for (int i = 0; ; i++)
        {
            try
            {
                return proceed();
            }
            catch (Exception e) when ( i < 5 )
            {
            }
        }
    }
}