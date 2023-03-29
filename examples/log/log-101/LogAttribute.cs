using Metalama.Framework.Aspects;

public class LogAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        Console.WriteLine( "Method started." );

        try
        {
            var result = meta.Proceed();

            Console.WriteLine( "Method succeeded." );

            return result;
        }
        catch ( Exception e )
        {
            Console.WriteLine( "Method failed: " + e.Message );

            throw;
        }
    }
}