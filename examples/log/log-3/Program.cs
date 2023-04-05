internal class Program
{
    private static void Main()
    {
        try
        {
            Calculator.Add( 1, 1 );
            Calculator.Divide( 0, 1 );
        }
        catch { }
    }

   
}