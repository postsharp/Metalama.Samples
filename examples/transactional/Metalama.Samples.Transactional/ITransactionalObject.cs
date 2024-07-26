using System.Diagnostics.CodeAnalysis;

namespace Metalama.Samples.Transactional;

public interface ITransactionalObject
{
    IMemoryTransactionContext TransactionContext { get; }
    TransactionalObjectId Id { get; }

    bool IsSameThan( ITransactionalObject? other );

    void NotifyStateChanged( in TransactionalObjectChangeNotification notification );
}

public static class TransactionalObjectExtensions
{
    public static TransactionalObjectId<T> GetTypedId<T>( this T obj )
        where T : ITransactionalObject
        => obj.Id.As<T>();

    public static T? GetTypedObject<T>( this IMemoryTransactionAccessor accessor,
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

