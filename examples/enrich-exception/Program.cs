internal class Program
{
    private static void Main()
    {
        try
        {
            Calculator.Fibonaci( 5 );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            Console.WriteLine( "   with:" );
            Console.WriteLine( e.Data["Context"] );
        }
    }

}
