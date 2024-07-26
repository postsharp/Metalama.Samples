namespace Metalama.Samples.Transactional;

public interface ITransactionalObjectState
{
    TransactionalObjectStateStatus Status { get; }

    void MakeReadOnly();
    
    ITransactionalObjectState ToDeleted();
    ITransactionalObjectState ToEditable();
}