using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

[assembly: AutoConfigureAwait]

class Program
{
    static async Task Main()
    {
        var client = new FakeHttpClient();
        Console.WriteLine(await MakeRequest(client));
    }

    private static async Task<string> MakeRequest(FakeHttpClient client) => await client.GetAsync("https://example.org");
}

class FakeHttpClient
{
    public async ValueTask<string> GetAsync(string url)
    {
        await Task.Yield();
        return "<html>";
    }
}