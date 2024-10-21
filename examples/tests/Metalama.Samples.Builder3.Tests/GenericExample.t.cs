using Metalama.Samples.Builder3;
namespace Metalama.Samples.Builder3.Tests.SimpleExample.GenericExample;
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
      var instance = new StringKeyedValue<T>(Value);
      return instance;
    }
  }
}