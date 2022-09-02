using Metalama.Framework.Aspects;

internal class IgnoreValuesAttribute : OverrideFieldOrPropertyAspect
{
    private readonly object?[] _ignoredValues;

    public IgnoreValuesAttribute( params object?[] values )
    {
        this._ignoredValues = values;
    }

    public override dynamic? OverrideProperty
    {
        get => meta.Proceed();
        set
        {
            foreach ( var ignoredValue in this._ignoredValues )
            {
                if ( value == ignoredValue )
                {
                    return;
                }

            }

            meta.Proceed();
        }
    }
}
