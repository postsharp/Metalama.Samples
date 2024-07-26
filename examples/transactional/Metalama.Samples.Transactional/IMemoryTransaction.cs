namespace Metalama.Samples.Transactional;

public interface IMemoryTransaction :  IDisposable
{
    MemoryTransactionOptions Options { get; }
    void Rollback();
    void Commit();

    ITransactionalObject GetObject( ITransactionalObject obj );
}