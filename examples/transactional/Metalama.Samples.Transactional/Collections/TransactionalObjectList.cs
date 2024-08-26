using System.Collections;
using System.Collections.Immutable;

namespace Metalama.Samples.Transactional;

public class TransactionalObjectList<T> : TransactionalObject, IList<T>
    where T : class, ITransactionalObject
{
    public TransactionalObjectList( ITransactionalMemoryAccessor memoryAccessor ) : base(
        memoryAccessor )
    {
    }

    public TransactionalObjectList( TransactionalObjectId id,
        ITransactionalMemoryAccessor memoryAccessor ) : base( memoryAccessor, id )
    {
    }

    private ImmutableList<TransactionalObjectId<T>> Items =>
        this.GetObjectState( false )
            .Items;

    private TransactionalObjectListState GetObjectState( bool editable ) =>
        (TransactionalObjectListState) this.MemoryAccessor.GetObjectState( this, editable );

    public IEnumerator<T> GetEnumerator() => this.Items
        .Select( id => (T) this.MemoryAccessor.GetObject( id ) ).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public void Add( T item )
    {
        var state = this.GetObjectState( true );
        state.Items = state.Items.Add( item.GetTypedId() );
    }

    public void Clear()
    {
        var state = this.GetObjectState( true );
        state.Items = ImmutableList<TransactionalObjectId<T>>.Empty;
    }


    public bool Contains( T item ) => this.Items.Contains( item.GetTypedId() );

    public void CopyTo( T[] array, int arrayIndex )
    {
        var index = arrayIndex;
        foreach ( var item in this.Items )
        {
            array[index] = this.MemoryAccessor.GetTypedObject( item )!;
            index++;
        }
    }

    public bool Remove( T item )
    {
        var state = this.GetObjectState( true );
        var id = item.GetTypedId();
        if ( state.Items.Contains( id ) )
        {
            state.Items = state.Items.Remove( id );
            return true;
        }
        else
        {
            return false;
        }
    }

    public int Count => this.Items.Count;
    bool ICollection<T>.IsReadOnly => false;
    public int IndexOf( T item ) => this.Items.IndexOf( item.GetTypedId() );

    public void Insert( int index, T item )
    {
        var state = this.GetObjectState( true );
        state.Items = state.Items.Insert( index, item.GetTypedId() );
    }

    public void RemoveAt( int index )
    {
        var state = this.GetObjectState( true );
        state.Items = state.Items.RemoveAt( index );
    }

    public T this[ int index ]
    {
        get => this.MemoryAccessor.GetTypedObject( this.Items[index] )!;
        set
        {
            var state = this.GetObjectState( true );
            state.Items = state.Items.SetItem( index, value.GetTypedId() );
        }
    }

    protected override ITransactionalObjectFactory TransactionalObjectFactory =>
        TransactionalObjectListFactory.Instance;

    protected class TransactionalObjectListState : TransactionalObjectState
    {
        public TransactionalObjectListState( TransactionalObjectId objectId ) : base( objectId )
        {
        }

        public ImmutableList<TransactionalObjectId<T>> Items =
            ImmutableList<TransactionalObjectId<T>>.Empty;
    }

    private class TransactionalObjectListFactory : ITransactionalObjectFactory
    {
        public static readonly TransactionalObjectListFactory Instance = new();

        public ITransactionalObjectState CreateInitialState( TransactionalObjectId id ) =>
            new TransactionalObjectListState( id );

        public ITransactionalObject CreateObject( TransactionalObjectId id,
            ITransactionalMemoryAccessor memoryAccessor ) =>
            new TransactionalObjectList<T>( id, memoryAccessor );

        public Type ObjectType => typeof(TransactionalObjectList<T>);
    }
}