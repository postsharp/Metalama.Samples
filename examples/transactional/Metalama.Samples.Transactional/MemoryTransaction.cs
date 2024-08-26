using System.Collections.Concurrent;
using System.Diagnostics;

namespace Metalama.Samples.Transactional;

internal abstract class MemoryTransaction : IMemoryTransaction, ITransactionalMemoryAccessor
{
    private readonly MemoryTransactionState _initialState;

    // This dictionary acts both as a cache and as a storage of modified states.
    private readonly ConcurrentDictionary<TransactionalObjectId, Node> _contextState = new();

    private readonly MemoryTransactionOptions _options;

    private bool _isDisposed;

    protected MemoryTransaction( MemoryTransactionOptions options,
        MemoryTransactionState initialState )
    {
        this._options = options;
        this._initialState = initialState;
        this.TransactionInfo = new MemoryTransactionInfo( options );
    }

    protected abstract IMemoryTransactionContext Context { get; }
    public IMemoryTransactionInfo TransactionInfo { get; }

    public MemoryTransactionOptions Options => this._options;

    public void RegisterObject( ITransactionalObject obj, ITransactionalObjectState state )
    {
        this.CheckNotDisposed();

        Debug.Assert( state.Status == TransactionalObjectStateStatus.Editable );

        if ( !this._contextState.TryAdd( obj.Id, new Node( obj, null, state ) ) )
        {
            throw new InvalidOperationException(
                "There is already a state object for this originator." );
        }
    }

    public void DeleteObject( ITransactionalObject obj )
    {
        this.CheckNotDisposed();

        var node = this.GetNode( obj );

        var spinWait = new SpinWait();

        while ( true )
        {
            var currentState = node.CurrentState;

            if ( currentState.Status == TransactionalObjectStateStatus.Deleted )
            {
                throw new ObjectDisposedException( obj.GetType().Name );
            }


            var deletedCopy = currentState.ToDeleted();

            if ( Interlocked.CompareExchange( ref node.CurrentState, deletedCopy, currentState ) ==
                 currentState )
            {
                return;
            }

            spinWait.SpinOnce();
        }
    }


    public ITransactionalObjectState GetObjectState( ITransactionalObject obj, bool editable )
    {
        this.CheckNotDisposed();

        var node = this.GetNode( obj );

        if ( editable && node.CurrentState.Status != TransactionalObjectStateStatus.Editable )
        {
            var spinWait = new SpinWait();

            while ( true )
            {
                var currentState = node.CurrentState;

                if ( currentState.Status == TransactionalObjectStateStatus.Deleted )
                {
                    throw new ObjectDisposedException( obj.GetType().Name );
                }

                if ( currentState.Status == TransactionalObjectStateStatus.Editable )
                {
                    return currentState;
                }

                var editableCopy = currentState.ToEditable();

                if ( Interlocked.CompareExchange( ref node.CurrentState, editableCopy,
                         currentState ) ==
                     currentState )
                {
                    return editableCopy;
                }

                spinWait.SpinOnce();
            }
        }
        else
        {
            return node.CurrentState;
        }
    }

    public ITransactionalObject GetObject( TransactionalObjectId id )
    {
        var node = this.GetNode( id );

        if ( node.TransactionalObject == null )
        {
            Interlocked.CompareExchange( ref node.TransactionalObject,
                this.Context.CreateObject( id, this ), null );
        }

        return node.TransactionalObject;
    }

    private Node GetNode( ITransactionalObject obj ) => this.GetNode( obj.Id, obj );

    private Node GetNode( TransactionalObjectId id, ITransactionalObject? obj = null )
    {
        if ( !this._contextState.TryGetValue( id, out var node ) )
        {
            if ( this._initialState.Nodes.TryGetValue( id, out var sharedNode ) )
            {
                if ( obj == null && this._options.BindToExecutionContext )
                {
                    obj = sharedNode.ContextBoundObject;
                }

                node = new Node( obj, sharedNode.State, sharedNode.State );
                node = this._contextState.GetOrAdd( id, node );
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        if ( node.CurrentState.Status == TransactionalObjectStateStatus.Deleted )
        {
            throw new ObjectDisposedException( nameof(ITransactionalObject) );
        }

        return node;
    }


    private class Node
    {
        public ITransactionalObject? TransactionalObject;
        public readonly ITransactionalObjectState? InitialState;
        public volatile ITransactionalObjectState CurrentState;

        public Node(
            ITransactionalObject? transactionalObject,
            ITransactionalObjectState? initialState,
            ITransactionalObjectState currentState )
        {
            this.TransactionalObject = transactionalObject;
            this.InitialState = initialState;
            this.CurrentState = currentState;
        }
    }

    public void Dispose()
    {
        if ( !this._isDisposed )
        {
            this.Rollback();
        }
    }

    public void Rollback()
    {
        this.CheckNotDisposed();

        this._isDisposed = true;
        this.Context.OnTransactionClosed( this );
    }

    private void CheckNotDisposed()
    {
        if ( this._isDisposed )
        {
            throw new ObjectDisposedException( this.GetType().Name );
        }
    }

    public void Commit()
    {
        this.CheckNotDisposed();

        lock ( this.Context.Manager.CommitSync )
        {
            var nodes = this.Context.Manager.State.Nodes;

            foreach ( var ourNode in this._contextState )
            {
                var currentState = ourNode.Value.CurrentState;

                if ( nodes.TryGetValue( ourNode.Key, out var theirNode ) )
                {
                    // Check if the state has changed in the meantime.
                    if ( !ReferenceEquals( ourNode.Value.InitialState, theirNode.State )
                         && (this._options.RequireRepeatableReads || currentState.Status !=
                             TransactionalObjectStateStatus.ReadOnly) )
                    {
                        throw new TransactionCommitException(
                            $"The '{ourNode.Key.GetType().Name}' has changed." );
                    }

                    if ( currentState.Status != TransactionalObjectStateStatus.ReadOnly )
                    {
                        currentState.MakeReadOnly();
                        nodes = nodes.SetItem( ourNode.Key,
                            new TransactionalObjectSharedNode( currentState,
                                theirNode.ContextBoundObject ) );
                        theirNode.ContextBoundObject?.NotifyStateChanged(
                            new TransactionalObjectChangeNotification( theirNode.State,
                                currentState ) );
                    }
                }
                else
                {
                    // This is a new node.
                    currentState.MakeReadOnly();

                    var contextBoundOriginator = this._options.BindToExecutionContext
                        ? ourNode.Value.TransactionalObject
                        : null;
                    nodes = nodes.Add( ourNode.Key,
                        new TransactionalObjectSharedNode( currentState, contextBoundOriginator ) );
                }

                this.Context.Manager.State = new MemoryTransactionState( nodes );
            }


            this._isDisposed = true;
            this.Context.OnTransactionClosed( this );
        }
    }

    public abstract ITransactionalObject GetObject( ITransactionalObject obj );
}