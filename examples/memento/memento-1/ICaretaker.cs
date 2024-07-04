public interface ICaretaker
{
    bool CanUndo { get; }

    void Capture( IOriginator originator );

    void Undo();
}
