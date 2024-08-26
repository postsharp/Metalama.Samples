namespace Metalama.Samples.Transactional;

public record MemoryTransactionOptions
{
    public bool RequireRepeatableReads { get; init; }
    public bool RequireNewTransaction { get; init; }

    public bool BindToExecutionContext { get; init; }
}