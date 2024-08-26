namespace Metalama.Samples.Transactional;

/// <summary>
/// Exposes read-only information to the current <see cref="IMemoryTransaction"/>.
/// </summary>
public interface IMemoryTransactionInfo
{
    long TransactionId { get; }

    /// <summary>
    /// Gets a value indicating whether the transaction is bound to the <see cref="ExecutionContext"/>.
    /// </summary>
    bool IsContextBound { get; }
}