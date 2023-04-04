#pragma warning disable CS8618

namespace Metalama.Samples.Log104.Tests.FieldOrWrongType
{
    internal class Foo
    {
        TextWriter _logger;

        [Log]
        public void Bar() { }
    }
}
