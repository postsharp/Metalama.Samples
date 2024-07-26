namespace Metalama.Samples.Transactional;

internal class ContextBoundMemoryTransaction : MemoryTransaction
{
    public ContextBoundMemoryTransaction( MemoryTransactionOptions options,
        ContextBoundTransactionContext context ) : base( options, context.Manager.State )
    {
        this.Context = context;
    }

    public override IMemoryTransactionContextImpl Context { get; }

    public override ITransactionalObject GetObject( ITransactionalObject obj )
        => this.Context.GetObject( obj );
}