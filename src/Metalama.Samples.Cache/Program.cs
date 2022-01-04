// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

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