using System.ComponentModel.DataAnnotations;

namespace Metalama.Samples.Builder2.Tests.DerivedGenericType;

[GenerateBuilder]
public class StringKeyedValue<T>
{
    [Required] public T Value { get; }
}

public class TaggedKeyValue : StringKeyedValue<string>
{
    public string Tag { get; }
}