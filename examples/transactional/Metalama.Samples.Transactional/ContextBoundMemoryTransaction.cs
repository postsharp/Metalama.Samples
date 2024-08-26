namespace Metalama.Samples.Transactional;

internal class ContextBoundMemoryTransaction : MemoryTransaction
{
    public ContextBoundMemoryTransaction( MemoryTransactionOptions options,
        ContextBoundContext context ) : base( options, context.Manager.State )
    {
        this.Context = context;
    }

    protected override IMemoryTransactionContext Context { get; }

    public override ITransactionalObject GetObject( ITransactionalObject obj )
        => this.Context.GetObject( obj );
}