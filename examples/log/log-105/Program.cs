using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal static class Program
{
    private static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging( builder => builder.AddConsole().SetMinimumLevel( LogLevel.Trace ) )
            .AddSingleton<Calculator>()
            .BuildServiceProvider();

        var calculator = serviceProvider.GetService<Calculator>()!;
        
        try
        {
            calculator.Add( 1, 1 );
        }
        catch { }
    }

   
}