namespace Metalama.Samples.Transactional;

public readonly struct TransactionalObjectId : IEquatable<TransactionalObjectId>
{
    private static long _nextId;
    private readonly long _id;

    private TransactionalObjectId( long id, ITransactionalObjectFactory factory )
    {
        this._id = id;
        this.Factory = factory;
    }
    
    public ITransactionalObjectFactory Factory { get; }

    public bool IsNull => this._id == 0;

    internal static TransactionalObjectId New( ITransactionalObjectFactory factory ) =>
        new(Interlocked.Increment( ref _nextId ), factory);

    public bool Equals(TransactionalObjectId other) => this._id == other._id;

    public override bool Equals(object? obj) => obj is TransactionalObjectId other && this.Equals(other);

    public override int GetHashCode() => this._id.GetHashCode();

    public override string ToString() => $"{{{this.Factory.ObjectType} Id={this._id:x}}}";

    public TransactionalObjectId<T> As<T>() where T : ITransactionalObject
    {
        if ( this.Factory.ObjectType != typeof(T) )
        {
            throw new ArgumentOutOfRangeException( nameof(T), "Type mismatch.");
        }

        return new TransactionalObjectId<T>( this );
    }
}


public readonly struct TransactionalObjectId<T> : IEquatable<TransactionalObjectId<T>>, IEquatable<TransactionalObjectId>
where T : ITransactionalObject
{
    private readonly TransactionalObjectId _underlying;

    internal TransactionalObjectId( in TransactionalObjectId underlying )
    {
        this._underlying = underlying;
    }

    public ITransactionalObjectFactory Factory => this._underlying.Factory;

    public bool IsNull => this._underlying.IsNull;

    internal static TransactionalObjectId<T> New(ITransactionalObjectFactory factory)
    {
        if ( factory.ObjectType != typeof(T) )
        {
            throw new ArgumentOutOfRangeException( nameof(factory), "Type mismatch.");
        }
        
        return new TransactionalObjectId<T>( TransactionalObjectId.New( factory ) );
    }

    public bool Equals( TransactionalObjectId<T> other )
    {
        return this._underlying.Equals(other._underlying);
    }
    
    public bool Equals( TransactionalObjectId other )
    {
        return this._underlying.Equals(other);
    }

    public override bool Equals(object? obj)
    {

        return obj switch
        {
            TransactionalObjectId<T> other => Equals( other ),
            TransactionalObjectId other => Equals( other ),
            _ => false
        };
    }
    
    public override int GetHashCode()
    {
        return this._underlying.GetHashCode();
    }

    public override string ToString() => this._underlying.ToString();

    public static implicit operator TransactionalObjectId( TransactionalObjectId<T> typed ) =>
        typed._underlying;
}

