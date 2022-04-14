// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

internal class Program
{
    private static async Task Main()
    {
        var cts = new CancellationTokenSource( TimeSpan.FromSeconds( 1.5 ) );

        try
        {
            await C.MakeRequests( cts.Token );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( ex.Message );
        }
    }
}

[AutoCancellationToken]
internal class C
{
    public static async Task MakeRequests( CancellationToken ct )
    {
        var client = new FakeHttpClient();
        await MakeRequest( client );
        Console.WriteLine( "request 1 succeeded" );
        await MakeRequest( client );
        Console.WriteLine( "request 2 succeeded" );
    }

    private static async Task MakeRequest( FakeHttpClient client ) 
        => await client.GetAsync( "https://httpbin.org/delay/1" );
}

internal class FakeHttpClient
{
    public async Task GetAsync( string url, CancellationToken cancellationToken = default )
    {
        Console.WriteLine( $"Pretending to fetch {url}." );
        await Task.Delay( 1_000, cancellationToken );
    }
}