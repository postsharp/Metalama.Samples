using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
Startup.ConfigureServices( services );
var serviceProvider = services.BuildServiceProvider();

var performanceCounter = serviceProvider.GetRequiredService<PerformanceCounterManager>();

for ( var i = 0; i < 100; i++ )
{
    performanceCounter.IncrementCounter( "Foo" );
    Thread.Sleep( Random.Shared.Next(10) );
}

performanceCounter.UploadAndReset();