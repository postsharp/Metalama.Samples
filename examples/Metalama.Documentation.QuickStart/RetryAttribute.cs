﻿using Metalama.Framework.Aspects;

namespace Metalama.Documentation.QuickStart;

public class RetryAttribute : OverrideMethodAspect
{
    public int MaxAttempts { get; set; } = 5;

    public override dynamic? OverrideMethod()
    {
        for ( var i = 0;; i++ )
        {
            try
            {
                return meta.Proceed();
            }
            catch ( Exception e ) when ( i < this.MaxAttempts )
            {
                Console.WriteLine( $"{e.Message}. Retrying in 100 ms." );
                Thread.Sleep( 100 );
            }
        }
    }
}