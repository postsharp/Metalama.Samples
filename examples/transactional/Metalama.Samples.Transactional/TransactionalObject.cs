namespace Metalama.Samples.Transactional;

public abstract partial class TransactionalObject : ITransactionalObject
{
    private readonly TransactionalObjectId _id;

    // Creates a new instance.
    protected TransactionalObject( ITransactionalMemoryAccessor memoryAccessor )
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        var factory = this.TransactionalObjectFactory;

        if ( factory.ObjectType != this.GetType() )
        {
            throw new ArgumentOutOfRangeException( nameof(factory),
                $"The factory does not create objects of type {this.GetType().Name}." );
        }

        this.MemoryAccessor = memoryAccessor;
        this._id = TransactionalObjectId.New( factory );

        memoryAccessor.RegisterObject( this, factory.CreateInitialState( this._id ) );
    }

    // Creates an object for an existing instance.
    protected TransactionalObject( ITransactionalMemoryAccessor memoryAccessor,
        TransactionalObjectId id )
    {
        this.MemoryAccessor = memoryAccessor;
        this._id = id;
    }

    protected abstract ITransactionalObjectFactory TransactionalObjectFactory { get; }


    protected ITransactionalMemoryAccessor MemoryAccessor { get; }

    protected virtual void DeleteObject() => this.MemoryAccessor.DeleteObject( this );

    IMemoryTransactionInfo? ITransactionalObject.TransactionInfo =>
        this.MemoryAccessor.TransactionInfo;

    TransactionalObjectId ITransactionalObject.Id => this._id;

    public bool IsSameThan( ITransactionalObject? other ) =>
        other != null && other.Id.Equals( this._id );

    public virtual void NotifyStateChanged( in TransactionalObjectChangeNotification notification )
    {
    }
}