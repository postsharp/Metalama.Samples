using System;

namespace Caravela.Samples.LogParameters.Tests.NoParameter
{
    class Foo
    {
        [Log]
        void Bar()
        {
            var arguments = new object[] { };
            Console.WriteLine("Caravela.Samples.LogParameters.Tests.NoParameter.Foo.Bar() started", arguments);
            try
            {
                object result = null;
                Console.WriteLine(string.Format("Caravela.Samples.LogParameters.Tests.NoParameter.Foo.Bar()", arguments) + " returned " + result);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Caravela.Samples.LogParameters.Tests.NoParameter.Foo.Bar() failed: " + e, arguments);
                throw;
            }
        }
    }
}
