// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

namespace Metalama.Samples.DependencyInjection
{
    internal class Program
    {

        private static void Main()
        {
            // Configure the service container.
            var services = new ServiceCollection();
            services.AddSingleton<IConsole>(new ConsoleService());
            ServiceLocator.Current = services.BuildServiceProvider();

            // Use the service.
            var greeter = new Greeter();
            greeter.Greet();
        }
            
    }

    // Some class consuming a service.
    internal partial class Greeter
    {
        [Inject]
        private IConsole _console;

        public void Greet() => this._console.WriteLine("Hello, world.");

    }

    // Service interface.
    internal interface IConsole
    {
        void WriteLine(string text);
    }

    // Service implementation.
    internal class ConsoleService : IConsole
    {
        public void WriteLine(string text) => Console.WriteLine(text);
    }


    // This class emulates the standard ServiceCollection. 
    // We intentionally use the system one so that this sample can load in https://try.metalama.net.
    internal class ServiceCollection : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = new();

        public object? GetService( Type serviceType )
        {
            this._services.TryGetValue( serviceType, out var value);
            return value;
        }

        internal void AddSingleton<T>( T service ) 
            where T : notnull
        {
            this._services[typeof(T)] = service;
        }

        internal IServiceProvider BuildServiceProvider() => this;
       
    }

}