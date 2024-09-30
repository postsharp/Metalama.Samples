using Metalama.Samples.Builder2;

namespace ClassLibrary1Metalama.Samples.Builder2.Tests.SimpleExample.GenericExample;

[GenerateBuilder]
public partial class StringKeyedValue<T>
{
    public T Value { get; }
}