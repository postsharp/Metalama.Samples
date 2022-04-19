// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Framework.Aspects;

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