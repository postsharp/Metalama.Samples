using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1.5));
        try
        {
            await C.MakeRequests(cts.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

[AutoCancellationToken]
class C
{
    public static async Task MakeRequests(CancellationToken ct)
    {
        var client = new FakeHttpClient();
        await MakeRequest(client);
        Console.WriteLine("request 1 succeeded");
        await MakeRequest(client);
        Console.WriteLine("request 2 succeeded");
    }

    private static async Task MakeRequest(FakeHttpClient client) => await client.GetAsync("https://httpbin.org/delay/1");
}

class FakeHttpClient
{
    public async Task GetAsync(string url, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1_000, cancellationToken);
    }
}