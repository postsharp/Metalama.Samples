using Metalama.Samples.Builder1;
namespace ClassLibrary1Metalama.Samples.Builder1.Tests.SimpleExample.GenericExample;
[GenerateBuilder]
public partial class StringKeyedValue<T>
{
  public T Value { get; }
  private StringKeyedValue(T value)
  {
    Value = value;
  }
  public StringKeyedValue<T>.Builder ToBuilder()
  {
    return new StringKeyedValue<T>.Builder(this);
  }
  public class Builder
  {
    public Builder()
    {
    }
    protected internal Builder(StringKeyedValue<T> source)
    {
      Value = source.Value;
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