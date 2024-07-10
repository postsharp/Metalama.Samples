public interface ITransaction : IDisposable
{
    ITransactionMemento GetTransactionMemento( IOriginator originator );

    void Commit();

    void Rollback();
}