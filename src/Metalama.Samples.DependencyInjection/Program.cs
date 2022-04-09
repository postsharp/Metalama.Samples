// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Microsoft.Extensions.DependencyInjection;

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

    internal partial class Greeter
    {
        [Inject]
        private IConsole _console;

        public void Greet() => this._console.WriteLine("Hello, world.");

    }

    internal interface IConsole
    {
        void WriteLine(string text);
    }

    internal class ConsoleService : IConsole
    {
        public void WriteLine(string text) => Console.WriteLine(text);
    }

}