// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using System;

internal class Program
{
    private static int _attempts;

    private static void Main()
    {
        try
        {
            Foo();
        }
        catch { }
    }

    [Retry( Attempts = 10 )]
    private static int Foo()
    {
        _attempts++;
        Console.WriteLine( $"Just trying for the {_attempts}-th time." );

        throw new Exception();
    }
}