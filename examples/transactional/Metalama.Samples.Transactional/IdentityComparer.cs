namespace Metalama.Samples.Transactional;

internal class IdentityComparer<T> : IEqualityComparer<T>
    where T : class
{
    public static IdentityComparer<T> Instance { get; } = new();
    private IdentityComparer() { }

    public bool Equals( T? x, T? y )
        => object.ReferenceEquals( x, y );

    public int GetHashCode( T obj )
        => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode( obj );
}