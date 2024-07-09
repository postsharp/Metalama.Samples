using Metalama.Framework.Aspects;

[assembly:
    AspectOrder( AspectOrderDirection.RunTime, typeof(CacheAttribute), typeof(LogAttribute) )]

internal static class Program
{
    [Log]
    private static void Main()
    {
        try
        {
            Console.WriteLine( Add( 1, 1 ) );
            Console.WriteLine( Add( 1, 1 ) );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( ex );
        }
    }

    [Log]
    [Cache]
    private static int Add( int a, int b )
    {
        Console.WriteLine( "Thinking..." );

        return a + b;
    }
}