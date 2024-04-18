internal class RemoteCalculator
{
    private static int _attempts;

    [Retry]
    public void Nothing()
    {
        Console.WriteLine( "Hello void!" );
    }

    [Retry]
    public async Task NothingAsync()
    {
        await Task.CompletedTask;
    }

    [Retry]
    public int Add( int a, int b )
    {
        // Let's pretend this method executes remotely
        // and can fail for network reasons.

        Thread.Sleep( 10 );

        _attempts++;
        Console.WriteLine( $"Attempt #{_attempts}." );

        if ( _attempts <= 3 )
        {
            throw new InvalidOperationException();
        }

        Console.WriteLine( $"Succeeded." );

        return a + b;
    }

    [Retry]
    public async Task<int> AddAsync( int a, int b )
    {
        // Let's pretend this method executes remotely
        // and can fail for network reasons.

        await Task.Delay( 10 );

        _attempts++;
        Console.WriteLine( $"Attempt #{_attempts}." );

        if ( _attempts <= 3 )
        {
            throw new InvalidOperationException();
        }

        Console.WriteLine( $"Succeeded." );

        return a + b;
    }
}