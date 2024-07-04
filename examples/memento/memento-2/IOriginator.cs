public interface IOriginator
{
    IMemento Save();

    void Restore( IMemento memento );
}