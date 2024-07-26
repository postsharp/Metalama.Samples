namespace Metalama.Samples.Transactional;

public abstract partial class TransactionalObject : ITransactionalObject
{
    private readonly TransactionalObjectId _id;
    
    // Creates a new instance.
    protected TransactionalObject( IMemoryTransactionAccessor transactionAccessor )
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        var factory = this.TransactionalObjectFactory;
        
        if ( factory.ObjectType != this.GetType() )
        {
            throw new ArgumentOutOfRangeException( nameof(factory), $"The factory does not create objects of type {this.GetType().Name}." );
        }
        
        this.TransactionAccessor = transactionAccessor;
        this._id = TransactionalObjectId.New(factory);
        
        transactionAccessor.RegisterObject( this, factory.CreateInitialState( this._id ) );
    }
    
    // Creates an object for an existing instance.
    protected TransactionalObject( IMemoryTransactionAccessor transactionAccessor, TransactionalObjectId id )
    {
        this.TransactionAccessor = transactionAccessor;
        this._id = id;
    }
    
    protected abstract ITransactionalObjectFactory TransactionalObjectFactory { get; }

  
    protected IMemoryTransactionAccessor TransactionAccessor { get; }
    
    protected virtual void DeleteObject()
    {
        this.TransactionAccessor.DeleteObject( this );
    }

    IMemoryTransactionContext ITransactionalObject.TransactionContext => this.TransactionAccessor;
    TransactionalObjectId ITransactionalObject.Id => this._id;

    public bool IsSameThan( ITransactionalObject? other ) =>
        other != null && other.Id.Equals( this._id );

    public virtual void NotifyStateChanged( in TransactionalObjectChangeNotification notification )
    {
    }
}