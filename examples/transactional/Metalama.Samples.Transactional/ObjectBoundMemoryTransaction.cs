namespace Metalama.Samples.Transactional;

internal class ObjectBoundMemoryTransaction : MemoryTransaction, IMemoryTransactionContext
{
    public ObjectBoundMemoryTransaction( MemoryTransactionOptions options,
        MemoryTransactionManager manager ) : base( options, manager.State )
    {
        this.Manager = manager;
    }

    protected override IMemoryTransactionContext Context => this;

    IMemoryTransactionInfo IMemoryTransactionContext.TransactionInfo => this.TransactionInfo;

    public override ITransactionalObject GetObject( ITransactionalObject obj )
    {
        if ( obj.TransactionInfo == this.TransactionInfo )
        {
            return obj;
        }
        else
        {
            return this.GetObject( obj.Id );
        }
    }

    public MemoryTransactionManager Manager { get; }

    ITransactionalObject IMemoryTransactionContext.CreateObject( TransactionalObjectId id,
        ITransactionalMemoryAccessor transaction ) =>
        id.Factory.CreateObject( id, this );

    void IMemoryTransactionContext.OnTransactionClosed( IMemoryTransaction transaction ) { }
}