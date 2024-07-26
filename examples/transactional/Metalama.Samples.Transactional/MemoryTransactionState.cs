using System.Collections.Immutable;

namespace Metalama.Samples.Transactional;

internal record MemoryTransactionState(
    ImmutableDictionary<TransactionalObjectId, TransactionalObjectSharedNode> Nodes )
{
    public static MemoryTransactionState Empty = new MemoryTransactionState(
        ImmutableDictionary<TransactionalObjectId, TransactionalObjectSharedNode>.Empty );
}