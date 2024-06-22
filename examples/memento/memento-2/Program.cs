//using Metalama.Samples.Memento;

//Console.WriteLine( "Hello, World!" );

//public class Account
//{
//    public decimal Balance { get; private set; }

//    public void Deposit(decimal amount)
//    {
//        this.Balance += amount;
//    }

//    public void Withdraw(decimal amount)
//    {
//        if ( this.Balance - amount < 0 )
//        {
//            throw new InvalidOperationException( "Insufficient funds." );
//        }

//        this.Balance -= amount;
//    }
//}

//public class Account
//{
//    private Transaction? _topMostTransaction;
//    private decimal _balance;

//    public decimal Balance
//    {
//        get => this._balance;
//        set
//        {
//            if (MementoManager.CurrentTransaction != null && this._topMostTransaction != MementoManager.CurrentTransaction )
//            {
//                MementoManager.CurrentTransaction.AppendMemento( new TransactionMemento( this ) );
//                this._topMostTransaction = MementoManager.CurrentTransaction;
//            }

//            this._balance = value;
//        }
//    }

//    public void Deposit( decimal amount )
//    {
//        this.Balance += amount;
//    }

//    public void Withdraw( decimal amount )
//    {
//        if ( this.Balance - amount < 0 )
//        {
//            throw new InvalidOperationException( "Insufficient funds." );
//        }

//        this.Balance -= amount;
//    }

//    public void OnTransactionRollback(ITransactionMemento transactionMemento)
//    {
//        this._balance = ((TransactionMemento)transactionMemento).Balance;
//        this._topMostTransaction = this._topMostTransaction?.Parent;
//    }

//    public void OnTransactionCommit()
//    {
//        this._topMostTransaction = this._topMostTransaction?.Parent;
//    }

//    private class TransactionMemento : ITransactionMemento
//    {
//        public decimal Balance { get; set; }

//        public TransactionMemento( Account account )
//        {
//            this.Balance = account._balance;
//        }
//    }
//}

//public interface ITransactionMemento
//{
//}