internal class Program
{
    private static void Main()
    {
        try
        {
            Fibonaci( 5 );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            Console.WriteLine( "   with:" );
            Console.WriteLine( e.Data["Context"] );
        }
    }

    [EnrichException]
    private static int Fibonaci( int n )
    {
        if ( n < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof(n) );
        }

        if ( n == 0 )
        {
            return 0;
        }

        // Intentionally ommitting these lines to create an error.
        //else if (n == 1)
        //{
        //    return 1
        //}
        else
        {
            return Fibonaci( n - 1 ) + Fibonaci( n - 2 );
        }
    }
}