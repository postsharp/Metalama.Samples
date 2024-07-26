namespace Metalama.Samples.Transactional;

public class TransactionCommitException : Exception
{
    public TransactionCommitException( string message ) : base( message ) { }
    
}