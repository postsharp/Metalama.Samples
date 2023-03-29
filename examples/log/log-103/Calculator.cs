internal static class Calculator
{
    [Log]
    public static int Add( int a, int b )
    {
        if ( a == 0 )
        {
            throw new ArgumentOutOfRangeException( nameof(a) );
        }

        return a + b;
    }
}