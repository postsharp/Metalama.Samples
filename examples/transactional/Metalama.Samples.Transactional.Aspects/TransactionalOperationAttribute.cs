using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;

namespace Metalama.Samples.Transactional.Aspects;

public class TransactionalOperationAttribute : OverrideMethodAspect
{
    [IntroduceDependency]
    private readonly IMemoryTransactionFactory? _memoryTransactionManager;
    
    public bool RequireRepeatableReads { get; init; }
    public bool RequireNewTransaction { get; init; }

    public override dynamic? OverrideMethod()
    {
        using var transaction = this._memoryTransactionManager?.OpenTransaction(
            new MemoryTransactionOptions { RequireRepeatableReads = this.RequireRepeatableReads, RequireNewTransaction = this.RequireNewTransaction} );

        var result = meta.Proceed();
        
        transaction?.Commit();

        return result;
    }
}