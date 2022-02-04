﻿// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

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

        throw new InvalidOperationException();
    }
}