using Metalama.Framework.Aspects;

#pragma warning disable CA1305

public class LogAttribute : OverrideMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        Console.WriteLine( meta.Target.Method.ToDisplayString() + " started." );

        try
        {
            var result = meta.Proceed();

            Console.WriteLine( meta.Target.Method.ToDisplayString() + " succeeded." );

            return result;
        }
        catch ( Exception e )
        {
            Console.WriteLine( meta.Target.Method.ToDisplayString() + " failed: " + e.Message );

            throw;
        }
    }
}