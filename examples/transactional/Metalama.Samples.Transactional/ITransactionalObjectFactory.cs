namespace Metalama.Samples.Transactional;

public interface ITransactionalObjectFactory
{
    ITransactionalObjectState CreateInitialState( TransactionalObjectId id );

    ITransactionalObject CreateObject( TransactionalObjectId id,
        ITransactionalMemoryAccessor memoryAccessor );

    Type ObjectType { get; }
}