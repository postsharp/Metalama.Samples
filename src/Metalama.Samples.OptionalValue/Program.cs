var account = new Account { Name = null };
Console.WriteLine( $"Account name specified? {account.OptionalValues.Name.IsSpecified}" );


[OptionalValueType]
internal partial class Account
{
    public string? Name { get; set; }

    public Account? Parent { get; set; }

    public partial class Optional { }
}