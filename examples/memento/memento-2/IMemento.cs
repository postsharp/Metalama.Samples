public interface IMemento
{
    IOriginator Originator { get; }
}

public interface ITransactionMemento : IMemento
{
    void Commit();
}