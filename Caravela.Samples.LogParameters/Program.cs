// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

internal class Program
{
    private static void Main()
    {
        Add( 1, 1 );
    }

    [Log]
    private static int Add( int a, int b )
    {
        return a + b;
    }
}