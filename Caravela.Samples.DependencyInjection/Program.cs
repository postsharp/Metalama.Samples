using System;
using Caravela.Framework.Code;

namespace Caravela.Samples.DependencyInjection
{
    class Program
    {
        [Import]
        IGreetingService _service { get; set; }

        static void Main()
        {
            var program = new Program();
            program._service.Greet("World");
        }
    }

    interface IGreetingService
    {
        void Greet(string name);
    }

    class GreetingService : IGreetingService
    {
        public void Greet(string name) => Console.WriteLine($"Hello, {name}.");
    }

    class ServiceLocator : IServiceProvider
    {
        public static readonly IServiceProvider ServiceProvider = new ServiceLocator();

        public object GetService(Type serviceType) => new GreetingService();
        
    }
}
