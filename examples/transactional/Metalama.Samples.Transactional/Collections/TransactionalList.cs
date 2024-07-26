using System.Collections;
using System.Collections.Immutable;

namespace Metalama.Samples.Transactional;



public class TransactionalList<T> : TransactionalObject, IList<T>
{
    
    public TransactionalList(IMemoryTransactionAccessor transactionAccessor) : base( transactionAccessor )
    {
        CheckTypeArgument( transactionAccessor );
    }

    private TransactionalList( TransactionalObjectId id, IMemoryTransactionAccessor transactionAccessor ) : base( transactionAccessor, id )
    {
        CheckTypeArgument( transactionAccessor );
    }

    private static void CheckTypeArgument( IMemoryTransactionAccessor transactionAccessor )
    {
        if ( !transactionAccessor.IsContextBound && typeof(ITransactionalObject).IsAssignableFrom( typeof(T) ) )
        {
            throw new ArgumentOutOfRangeException( nameof(T),
                $"The type {typeof(T)} cannot implement the {nameof(TransactionalList<T>)} type because the object is bound to a transaction." );
        }
    }

    private ImmutableList<T> Items =>
        this.GetObjectState(false)
            .Items;

    private TransactionalListState GetObjectState(bool editable) => ((TransactionalListState) this.TransactionAccessor.GetObjectState( this, editable ));

    public IEnumerator<T> GetEnumerator() => this.Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public void Add( T item )
    {
        var state = this.GetObjectState( true );
        state.Items = state.Items.Add( item );
    }

    public void Clear()
    {
        var state = this.GetObjectState( true );
        state.Items = ImmutableList<T>.Empty;
    }


    public bool Contains( T item ) => this.Items.Contains( item );

    public void CopyTo( T[] array, int arrayIndex ) => this.Items.CopyTo( array, arrayIndex );

    public bool Remove( T item )
    {
        var state = this.GetObjectState( true );
        if ( state.Items.Contains( item ) )
        {
            state.Items = state.Items.Remove( item );
            return true;
        }
        else
        {
            return false;
        }
    }

    public int Count => this.Items.Count;
    bool ICollection<T>.IsReadOnly => false;
    public int IndexOf( T item ) => this.Items.IndexOf( item );

    public void Insert( int index, T item )
    {
        var state = this.GetObjectState( true );
        state.Items = state.Items.Insert( index, item );
    }

    public void RemoveAt( int index )
    {
        var state = this.GetObjectState( true );
        state.Items = state.Items.RemoveAt( index );
    }

    public T this[ int index ]
    {
        get => this.Items[index];
        set
        {
            var state = this.GetObjectState( true );
            state.Items = state.Items.SetItem( index, value );
        }
    }
    
    protected override ITransactionalObjectFactory TransactionalObjectFactory => TransactionalListFactory.Instance;

    protected class TransactionalListState : TransactionalObjectState
    {
        public TransactionalListState(TransactionalObjectId objectId) : base(objectId)
        {
        }

        public ImmutableList<T> Items = ImmutableList<T>.Empty;
    }

    private class TransactionalListFactory : ITransactionalObjectFactory
    {
        public static readonly TransactionalListFactory Instance = new();
        public ITransactionalObjectState CreateInitialState( TransactionalObjectId id ) =>
            new TransactionalListState( id );

        public ITransactionalObject CreateObject( TransactionalObjectId id,
            IMemoryTransactionAccessor transactionAccessor ) =>
            new TransactionalList<T>( id, transactionAccessor );

        public Type ObjectType => typeof(TransactionalList<T>);
    }

    
}