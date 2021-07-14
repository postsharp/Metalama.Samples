using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravela.Samples.Log.Tests.SimpleMethod
{
    class Foo
    {
        [Log]
        void Bar()
        {
            Console.WriteLine("Caravela.Samples.Log.Tests.SimpleMethod.Foo.Bar() started.");
            try
            {
                object result = null;
                Console.WriteLine("Caravela.Samples.Log.Tests.SimpleMethod.Foo.Bar() succeeded.");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Caravela.Samples.Log.Tests.SimpleMethod.Foo.Bar() failed: " + e.Message);
                throw;
            }
        }
    }
}
