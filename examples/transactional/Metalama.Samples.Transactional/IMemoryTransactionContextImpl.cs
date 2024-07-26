namespace Metalama.Samples.Transactional;

public interface IMemoryTransactionContext
{
    bool IsContextBound { get; }
}
internal interface IMemoryTransactionContextImpl : IMemoryTransactionContext
{
    MemoryTransactionManager Manager { get; }
    ITransactionalObject CreateObject( TransactionalObjectId id, IMemoryTransactionAccessor transaction );
    void OnTransactionClosed( IMemoryTransaction transaction );
    ITransactionalObject GetObject( ITransactionalObject obj );
}