using Metalama.Samples.Builder1;

namespace ClassLibrary1Metalama.Samples.Builder1.Tests.SimpleExample.GenericExample;

[GenerateBuilder]
public partial class StringKeyedValue<T>
{
    public T Value { get; }
}