namespace Metalama.Samples.Transactional;

internal class MemoryTransactionInfo : IMemoryTransactionInfo
{
    private static long _nextId;
    private readonly MemoryTransactionOptions _options;

    public MemoryTransactionInfo( MemoryTransactionOptions options )
    {
        this._options = options;
    }

    public long TransactionId { get; } = Interlocked.Increment( ref _nextId );

    public bool IsContextBound => this._options.BindToExecutionContext;
}