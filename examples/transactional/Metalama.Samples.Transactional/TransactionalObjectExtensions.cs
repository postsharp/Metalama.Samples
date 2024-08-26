namespace Metalama.Samples.Transactional;

public static class TransactionalObjectExtensions
{
    public static TransactionalObjectId<T> GetTypedId<T>( this T obj )
        where T : ITransactionalObject
        => obj.Id.As<T>();

    public static T? GetTypedObject<T>( this ITransactionalMemoryAccessor accessor,
        TransactionalObjectId<T> id ) where T : class, ITransactionalObject
    {
        if ( !id.IsNull )
        {
            return (T) accessor.GetObject( (TransactionalObjectId) id );
        }
        else
        {
            return null;
        }
    }
}