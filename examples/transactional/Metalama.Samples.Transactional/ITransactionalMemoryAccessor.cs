namespace Metalama.Samples.Transactional;

public interface ITransactionalMemoryAccessor //: IMemoryTransactionContext
{
    IMemoryTransactionInfo? TransactionInfo { get; }

    void RegisterObject( ITransactionalObject obj, ITransactionalObjectState state );
    void DeleteObject( ITransactionalObject obj );
    ITransactionalObjectState GetObjectState( ITransactionalObject obj, bool editable );
    ITransactionalObject GetObject( TransactionalObjectId id );
}