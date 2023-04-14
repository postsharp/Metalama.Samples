var account = new Account { Name = null };
Console.WriteLine( $"Account name specified? {account.OptionalValues.Name.IsSpecified}" );