using Metalama.Framework.Aspects;

public class RetryAttribute : OverrideMethodAspect
{
    public int Attempts { get; set; } = 5;

    public override dynamic? OverrideMethod()
    {
        for ( var i = 0;; i++ )
        {
            try
            {
                return meta.Proceed();
            }
            catch ( Exception e ) when ( i < this.Attempts )
            {
                Console.WriteLine( e.Message + " Retrying." );
            }
        }
    }
}