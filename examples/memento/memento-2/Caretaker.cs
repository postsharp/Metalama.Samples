public class Caretaker : ICaretaker
{
    private readonly Stack<IMemento> _mementos = new Stack<IMemento>();

    public void Capture(IOriginator originator)
    {
        this._mementos.Push( originator.Save() );
    }

    public void Undo()
    {
        if ( this._mementos.Count > 0 )
        {
            var memento = this._mementos.Pop();
            memento.Originator.Restore(memento);
        }
    }

    public bool CanUndo => this._mementos.Count > 0;
}

public interface ICaretaker
{
    bool CanUndo { get; }

    void Capture( IOriginator originator );

    void Undo();
}

public interface IMemento
{
    IOriginator Originator { get; }
}

public interface IOriginator
{
    IMemento Save();

    void Restore( IMemento memento );
}