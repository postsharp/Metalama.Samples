#pragma warning disable CS8618

namespace Metalama.Samples.Log4.Tests.FieldOrWrongType;

internal class Foo
{
    private TextWriter _logger;

    [Log]
    public void Bar() { }
}