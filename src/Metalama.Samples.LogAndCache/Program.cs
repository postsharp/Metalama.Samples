// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

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