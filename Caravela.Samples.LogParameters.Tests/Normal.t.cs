using System;

namespace Caravela.Samples.LogParameters.Tests.Normal
{
    class Foo
    {
        [Log]
        void Bar(int a, string b)
        {
            var arguments = new object[] { a, b };
            Console.WriteLine("Caravela.Samples.LogParameters.Tests.Normal.Foo.Bar(a = {0}, b = {1}) started", arguments);
            try
            {
                object result = null;
                Console.WriteLine(string.Format("Caravela.Samples.LogParameters.Tests.Normal.Foo.Bar(a = {0}, b = {1})", arguments) + " returned " + result);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Caravela.Samples.LogParameters.Tests.Normal.Foo.Bar(a = {0}, b = {1}) failed: " + e, arguments);
                throw;
            }
        }
    }
}
