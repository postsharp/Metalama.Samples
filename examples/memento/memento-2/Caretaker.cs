public class Caretaker : ICaretaker
{
    private readonly Stack<IMemento> _mementos = new Stack<IMemento>();

    public ITransaction? CurrentTransaction { get; private set; }

    public void Capture( IOriginator originator )
    {
        this._mementos.Push( originator.Capture() );
    }

    public void Undo()
    {
        if ( this._mementos.Count > 0 )
        {
            var memento = this._mementos.Pop();
            memento.Originator.Restore( memento );
        }
    }

    public bool CanUndo => this._mementos.Count > 0;

    public ITransaction BeginTransaction( IOriginator originator )
    {
        if (this.CurrentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        return this.CurrentTransaction = new Transaction( this );
    }

    private void EndTransaction(ITransaction transaction)
    {
        if ( this.CurrentTransaction != transaction )
        {
            throw new InvalidOperationException("The transaction is not the current transaction.");
        }

        this.CurrentTransaction = null;
    }

    private class Transaction : ITransaction
    {
        private readonly Caretaker _owner;
        private readonly Dictionary<IOriginator, ITransactionMemento> _transactionMementos;

        public Transaction(Caretaker owner)
        {
            this._owner = owner;
            this._transactionMementos = [];
        }

        public void Commit()
        {
            foreach ( var transactionMemento in this._transactionMementos.Values )
            {
                transactionMemento.Commit();
            }

            this._owner.EndTransaction( this );
        }

        public void Dispose()
        {
            this._owner.EndTransaction( this );
        }

        public ITransactionMemento GetTransactionMemento( IOriginator originator )
        {
            if ( !this._transactionMementos.TryGetValue( originator, out var transactionMemento ) )
            {
                transactionMemento = (ITransactionMemento)originator.Capture();
                this._transactionMementos[originator] = transactionMemento;
            }

            return transactionMemento;
        }

        public void Rollback()
        {
            this._owner.EndTransaction( this );
        }
    }
}
