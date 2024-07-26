namespace Metalama.Samples.Transactional;

public interface IMemoryTransactionAccessor : IMemoryTransactionContext
{
    void RegisterObject( ITransactionalObject obj, ITransactionalObjectState state );
    void DeleteObject( ITransactionalObject obj );
    ITransactionalObjectState GetObjectState( ITransactionalObject obj, bool editable );
    ITransactionalObject GetObject( TransactionalObjectId id );
}