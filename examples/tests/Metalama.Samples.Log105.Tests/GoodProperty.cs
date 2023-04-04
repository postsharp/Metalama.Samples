
using Microsoft.Extensions.Logging;

namespace Metalama.Samples.Log104.Tests.GoodProperty
{
    internal class Foo
    {
        private readonly ILogger _logger;

        public Foo( ILogger logger )
        {
            this._logger = logger;
        }

        [Log]
        public void Method() { }
    }
}
