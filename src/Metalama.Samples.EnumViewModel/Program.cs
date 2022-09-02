var viewModel = new VisibilityViewModel( Visibility.Collapsed );
Console.WriteLine( $"IsCollapsed={viewModel.IsCollapsed}" );

internal enum Visibility
{
    Visible,
    Hidden,
    Collapsed

}

[Flags]
internal enum StringOptions
{
    None,
    ToUpperCase = 1,
    RemoveSpace = 2,
    Trim = 4
}

// Some boilerplate is still necessary with this version of Metalama because
// we cannot generate new types and constructors yet.

[EnumViewModel]
internal partial class VisibilityViewModel 
{
    private readonly Visibility _value;

    public VisibilityViewModel( Visibility value )
    {
        this._value = value;
    }
}

[EnumViewModel]
internal partial class StringOptionsViewModel
{
    private readonly StringOptions _value;

    public StringOptionsViewModel( StringOptions value )
    {
        this._value = value;
    }
}

