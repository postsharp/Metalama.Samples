// Warning SING01 on `PublicConstructorSingleton`: `The constructor PublicConstructorSingleton.PublicConstructorSingleton() of the singleton class PublicConstructorSingleton has to be private.`
[Singleton]
internal class PrivateConstructorSingleton
{
    private PrivateConstructorSingleton()
    {
    }
    public PrivateConstructorSingleton Instance { get; } = new();
}
[Singleton]
internal class PublicConstructorSingleton
{
    public PublicConstructorSingleton()
    {
    }
    public PublicConstructorSingleton Instance { get; } = new();
}