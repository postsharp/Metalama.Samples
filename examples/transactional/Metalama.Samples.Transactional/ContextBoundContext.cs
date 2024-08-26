using System.Collections.Concurrent;

namespace Metalama.Samples.Transactional;

internal class ContextBoundContext : ITransactionalMemoryAccessor, IMemoryTransactionContext
{
    private readonly AsyncLocal<MemoryTransaction?> _currentContext = new();

    // A cache for fast access because access to ImmutableDictionary is slow.
    private readonly ConcurrentDictionary<TransactionalObjectId, ITransactionalObjectState>
        _stateCache = new();

    // The list of all transactional objects.
    private readonly ConcurrentDictionary<TransactionalObjectId, ITransactionalObject> _objects =
        new();


    public ContextBoundContext( MemoryTransactionManager manager )
    {
        this.Manager = manager;
        this.Manager.StateChanged += this.OnStateChanged;
    }

    public MemoryTransactionManager Manager { get; }

    public MemoryTransaction? CurrentTransaction
    {
        get => this._currentContext.Value;
        set => this._currentContext.Value = value;
    }


    private void OnStateChanged() => this._stateCache.Clear();

    private Exception NotAllowedException() =>
        throw new InvalidOperationException(
            "This operation is not allowed out of a transaction." );

    IMemoryTransactionInfo? ITransactionalMemoryAccessor.TransactionInfo =>
        this.CurrentTransaction?.TransactionInfo;

    IMemoryTransactionInfo? IMemoryTransactionContext.TransactionInfo =>
        this.CurrentTransaction?.TransactionInfo;

    public void RegisterObject( ITransactionalObject obj, ITransactionalObjectState state )
    {
        if ( !this._objects.TryAdd( obj.Id, obj ) )
        {
            throw new InvalidOperationException();
        }

        var transaction = this._currentContext.Value ?? throw this.NotAllowedException();
        transaction.RegisterObject( obj, state );
    }

    public void DeleteObject( ITransactionalObject obj )
    {
        var transaction = this._currentContext.Value ?? throw this.NotAllowedException();
        transaction.DeleteObject( obj );

        if ( !this._objects.TryRemove( obj.Id, out var removed ) || removed != obj )
        {
            throw new InvalidOperationException();
        }
    }

    public ITransactionalObjectState GetObjectState( ITransactionalObject obj, bool editable )
    {
        var transaction = this._currentContext.Value;

        if ( transaction == null )
        {
            if ( editable )
            {
                throw this.NotAllowedException();
            }

            return this.GetTransactionlessState( obj );
        }
        else
        {
            return transaction.GetObjectState( obj, editable );
        }
    }

    private ITransactionalObjectState GetTransactionlessState( ITransactionalObject originator )
    {
        var objectId = originator.Id;

        if ( !this._stateCache.TryGetValue( objectId, out var state ) )
        {
            if ( !this.Manager.State.Nodes.TryGetValue( objectId, out var sharedNode ) )
            {
                throw new KeyNotFoundException();
            }

            state = this._stateCache.GetOrAdd( objectId, sharedNode.State );
        }


        if ( state.Status == TransactionalObjectStateStatus.Deleted )
        {
            throw new ObjectDisposedException( originator.GetType().Name );
        }

        return state;
    }

    public ITransactionalObject GetObject( TransactionalObjectId id )
    {
        if ( this._objects.TryGetValue( id, out var obj ) )
        {
            return obj;
        }

        return this.CreateObject( id, this );
    }

    public bool IsContextBound => true;

    public ITransactionalObject CreateObject( TransactionalObjectId id,
        ITransactionalMemoryAccessor transaction ) => id.Factory.CreateObject( id, this );

    public void OnTransactionClosed( IMemoryTransaction transaction ) =>
        this.CurrentTransaction = null;

    public ITransactionalObject GetObject( ITransactionalObject obj )
    {
        if ( obj.TransactionInfo?.IsContextBound == true )
        {
            return obj;
        }
        else
        {
            return this.GetObject( obj.Id );
        }
    }
}