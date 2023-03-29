public struct OptionalValue<T>
{
    public bool IsSpecified { get; private set; }

    public T Value { get; }

    public OptionalValue( T value )
    {
        this.Value = value;
        this.IsSpecified = true;
    }
}