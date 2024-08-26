namespace Metalama.Samples.Transactional;

internal readonly struct TransactionalObjectSharedNode
{
    public TransactionalObjectSharedNode(
        ITransactionalObjectState state,
        ITransactionalObject? contextBoundObject )
    {
        this.ContextBoundObject = contextBoundObject;
        this.State = state;
    }

    public ITransactionalObjectState State { get; }
    public ITransactionalObject? ContextBoundObject { get; }
}