public class Calculator
{
    public int InvocationCounts { get; private set; }

    public int Add( int a, int b )
    {
        Console.WriteLine( "Thinking..." );
        this.InvocationCounts++;
        return a + b;
    }
}