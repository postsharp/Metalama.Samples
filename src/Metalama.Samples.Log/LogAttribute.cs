// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using System;

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