using Metalama.Samples.Builder2;
namespace ClassLibrary1Metalama.Samples.Builder2.Tests.SimpleExample.GenericExample;
#pragma warning disable CS8618 //  Non-nullable property must contain a non-null value when exiting constructor.
[GenerateBuilder]
public partial class StringKeyedValue<T>
{
  public T Value { get; }
  protected StringKeyedValue(T value)
  {
    Value = value;
  }
  public virtual StringKeyedValue<T>.Builder ToBuilder()
  {
    var builder = new StringKeyedValue<T>.Builder();
    builder.Value = Value;
    return builder;
  }
  public class Builder
  {
    public Builder()
    {
    }
    private T _value = default !;
    public T Value
    {
      get
      {
        return _value;
      }
      set
      {
        _value = value;
      }
    }
    public StringKeyedValue<T> Build()
    {
      return new StringKeyedValue<T>(Value)!;
    }
  }
}