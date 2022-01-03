// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using System;

[assembly: AspectOrder( typeof(CacheAttribute), typeof(LogAttribute) )]

internal class Program
{
    [Log]
    private static void Main()
    {
        try
        {
            Add( 1, 1 );
            Add( 1, 1 );
        }
        catch { }
    }

    [Log]
    [Cache]
    private static int Add( int a, int b )
    {
        Console.WriteLine( "Thinking..." );

        return a + b;
    }
}