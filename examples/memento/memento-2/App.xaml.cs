using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NameGenerator;
using NameGenerator.Generators;
using System.Windows;

namespace Sample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? Host { get; private set; }

    public App()
    {
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices( ( context, services ) =>
            {
                // Add windows
                services.AddSingleton<MainWindow>();

                // Add root view models
                services.AddSingleton<MainViewModel>();

                // Add services
                services.AddSingleton<ICaretaker, Caretaker>();
                services.AddSingleton<IDataSource, DataSource>();
                services.AddSingleton<GeneratorBase, RealNameGenerator>();
            } )
            .Build();
    }

    protected override async void OnStartup( StartupEventArgs e )
    {
        await Host!.StartAsync();

        var mainWindow = Host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup( e );
    }

    protected override async void OnExit( ExitEventArgs e )
    {
        await Host!.StopAsync();
        base.OnExit( e );
    }
}
