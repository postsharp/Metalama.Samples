internal class Program
{
    private static int _attempts;

    private static void Main()
    {
        try
        {
            Foo();
        }
        catch { }
    }

    [Retry( Attempts = 10 )]
    private static int Foo()
    {
        _attempts++;
        Console.WriteLine( $"Just trying for the {_attempts}-th time." );

        throw new InvalidOperationException();
    }
}