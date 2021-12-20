// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using System;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine( "Calling a first time." );
        Add( 1, 1 );
        Console.WriteLine( "Calling a second time." );
        Add( 1, 1 );
        Console.WriteLine( "Completed." );
    }

    [Cache]
    private static int Add( int a, int b )
    {
        Console.WriteLine( "Thinking..." );

        return a + b;
    }
}