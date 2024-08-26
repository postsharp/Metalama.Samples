namespace Metalama.Samples.Transactional;

public record struct TransactionalObjectChangeNotification(
    ITransactionalObjectState PreviousState,
    ITransactionalObjectState NewState );