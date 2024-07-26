namespace Metalama.Samples.Transactional.Aspects;

[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
public class NotTransactionalAttribute : Attribute;