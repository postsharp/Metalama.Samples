using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal static class Program
{
    private static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging( builder => builder.AddConsole().SetMinimumLevel( LogLevel.Trace ) )
            .AddSingleton<LoginService>()
            .BuildServiceProvider();

        var service = serviceProvider.GetService<LoginService>()!;
        service.GetSaltedHash( "adam", "eva", "saltzburg" );
        service.VerifyPassword( "adam", "eva" );
        
    }

   
}