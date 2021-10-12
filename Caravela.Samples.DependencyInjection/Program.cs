using System;

namespace Caravela.Samples.DependencyInjection
{
    internal class Program
    {
        [Import] 
        private IGreetingService _service { get; set; }

        private static void Main()
        {
            var program = new Program();
            program._service.Greet("World");
        }
    }

    internal interface IGreetingService
    {
        void Greet(string name);
    }

    internal class GreetingService : IGreetingService
    {
        public void Greet(string name) => Console.WriteLine($"Hello, {name}.");
    }

    internal class ServiceLocator : IServiceProvider
    {
        public static readonly IServiceProvider ServiceProvider = new ServiceLocator();

        public object GetService(Type serviceType) => new GreetingService();
    }
}