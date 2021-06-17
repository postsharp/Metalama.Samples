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
        void Bar() { }
    }
}
