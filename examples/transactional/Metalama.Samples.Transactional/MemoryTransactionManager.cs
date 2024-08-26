namespace Metalama.Samples.Transactional;

internal class MemoryTransactionManager : IMemoryTransactionFactory
{
    private readonly ContextBoundContext _contextBoundContext;
    private MemoryTransactionState _state = MemoryTransactionState.Empty;

    internal MemoryTransactionState State
    {
        get => this._state;
        set
        {
            this._state = value;
            this.StateChanged?.Invoke();
        }
    }

    public event Action? StateChanged;

    public MemoryTransactionManager()
    {
        this._contextBoundContext = new ContextBoundContext( this );
    }

    public object CommitSync { get; } = new();

    public IMemoryTransaction? OpenTransaction( MemoryTransactionOptions options )
    {
        var currentTransaction = this._contextBoundContext.CurrentTransaction;

        if ( currentTransaction != null )
        {
            if ( options.RequireNewTransaction )
            {
                throw new NotSupportedException(
                    "A transaction is already in progress, but a new one is requested. Nested transactions are not supported." );
            }

            if ( !currentTransaction.Options.RequireRepeatableReads &&
                 options.RequireRepeatableReads )
            {
                throw new NotSupportedException(
                    "The current transaction does not require repeatable reads." );
            }

            return null;
        }

        MemoryTransaction transaction;


        if ( options.BindToExecutionContext )
        {
            transaction = new ContextBoundMemoryTransaction( options, this._contextBoundContext );
            this._contextBoundContext.CurrentTransaction = transaction;
        }
        else
        {
            transaction = new ObjectBoundMemoryTransaction( options, this );
        }

        return transaction;
    }
}