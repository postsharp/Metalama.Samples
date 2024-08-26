namespace Metalama.Samples.Transactional;

internal interface IMemoryTransactionContext
{
    IMemoryTransactionInfo? TransactionInfo { get; }
    MemoryTransactionManager Manager { get; }

    ITransactionalObject CreateObject( TransactionalObjectId id,
        ITransactionalMemoryAccessor transaction );

    void OnTransactionClosed( IMemoryTransaction transaction );
    ITransactionalObject GetObject( ITransactionalObject obj );
}