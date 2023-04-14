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