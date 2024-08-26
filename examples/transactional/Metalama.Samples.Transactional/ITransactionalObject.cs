using System.Diagnostics.CodeAnalysis;

namespace Metalama.Samples.Transactional;

public interface ITransactionalObject
{
    IMemoryTransactionInfo? TransactionInfo { get; }

    TransactionalObjectId Id { get; }

    bool IsSameThan( ITransactionalObject? other );

    void NotifyStateChanged( in TransactionalObjectChangeNotification notification );
}