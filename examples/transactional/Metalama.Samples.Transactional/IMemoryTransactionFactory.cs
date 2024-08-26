namespace Metalama.Samples.Transactional;

public interface IMemoryTransactionFactory
{
    IMemoryTransaction? OpenTransaction( MemoryTransactionOptions options );
}