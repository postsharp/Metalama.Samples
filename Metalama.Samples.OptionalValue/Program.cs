// This is an open-source Metalama example. See https://github.com/postsharp/Metalama.Samples for more.

using Metalama.Samples.OptionalValue;

[OptionalValueType]
internal class Account
{
    public string Name { get; set; }

    public Account? Parent { get; set; }

    public class Optional { }
}