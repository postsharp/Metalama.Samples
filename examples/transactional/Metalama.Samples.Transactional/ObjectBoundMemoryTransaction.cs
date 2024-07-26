namespace Metalama.Samples.Transactional;

internal class ObjectBoundMemoryTransaction : MemoryTransaction, IMemoryTransactionContextImpl
{
    public ObjectBoundMemoryTransaction(MemoryTransactionOptions options, MemoryTransactionManager manager) : base(options, manager.State)
    {
        this.Manager = manager;
    }

    public override IMemoryTransactionContextImpl Context => this;

    public override ITransactionalObject GetObject( ITransactionalObject obj )
    {
        if ( obj.TransactionContext == this )
        {
            return obj;
        }
        else
        {
            return this.GetObject( obj.Id );
        }
    }

    public MemoryTransactionManager Manager { get; }

    ITransactionalObject IMemoryTransactionContextImpl.CreateObject( TransactionalObjectId id,
        IMemoryTransactionAccessor transaction ) =>
        id.Factory.CreateObject( id, this );

    void IMemoryTransactionContextImpl.OnTransactionClosed( IMemoryTransaction transaction ) { }
}