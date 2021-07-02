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
            global::System.Console.WriteLine("Caravela.Samples.Log.Tests.SimpleMethod.Foo.Bar() started.");
            try
            {
                global::Caravela.Framework.Aspects.__Void result;
                global::System.Console.WriteLine("Caravela.Samples.Log.Tests.SimpleMethod.Foo.Bar() succeeded.");
                return;
            }
            catch (global::System.Exception e)
            {
                global::System.Console.WriteLine("Caravela.Samples.Log.Tests.SimpleMethod.Foo.Bar() failed: " + e.Message);
                throw;
            }
        }
    }
}