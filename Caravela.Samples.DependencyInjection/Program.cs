using System;
using Caravela.Framework.Code;

namespace Caravela.Samples.DependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    static class ServiceLocator
    {
        public static IServiceProvider ServiceProvider;
    }
}
