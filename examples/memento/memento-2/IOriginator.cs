public interface IOriginator
{
    IMemento Capture();

    void Restore( IMemento memento );
}