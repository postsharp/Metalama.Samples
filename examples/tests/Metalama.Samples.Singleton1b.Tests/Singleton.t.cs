// Warning SING02 on `new()`: `The constructor 'DifferentlyNamedPropertySingleton.DifferentlyNamedPropertySingleton()' cannot be referenced by DifferentlyNamedPropertySingleton.Value.`
// Warning SING02 on `new()`: `The constructor 'MethodInsteadOfPropertySingleton.MethodInsteadOfPropertySingleton()' cannot be referenced by MethodInsteadOfPropertySingleton.GetInstance().`
// Warning SING02 on `new()`: `The constructor 'CallingConstructorFromGetterSingleton.CallingConstructorFromGetterSingleton()' cannot be referenced by CallingConstructorFromGetterSingleton.Instance.get.`
[Singleton]
internal class CorrectSingleton
{
    private CorrectSingleton()
    {
    }
    public CorrectSingleton Instance { get; } = new();
}
[Singleton]
internal class DifferentlyNamedPropertySingleton
{
    private DifferentlyNamedPropertySingleton()
    {
    }
    public DifferentlyNamedPropertySingleton Value { get; } = new();
}
[Singleton]
internal class MethodInsteadOfPropertySingleton
{
    private MethodInsteadOfPropertySingleton()
    {
    }
    public MethodInsteadOfPropertySingleton GetInstance() => new();
}
[Singleton]
internal class CallingConstructorFromGetterSingleton
{
    private CallingConstructorFromGetterSingleton()
    {
    }
    public CallingConstructorFromGetterSingleton Instance { get => new(); }
}