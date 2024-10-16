using Polly;
using Polly.Registry;

internal class ResiliencePipelineFactory : IResiliencePipelineFactory
{
    private readonly ResiliencePipelineRegistry<StrategyKind> _registry = new();

    public ResiliencePipeline GetPipeline( StrategyKind strategyKind )
        => this._registry.GetOrAddPipeline( strategyKind, ( builder, context ) =>
        {
            switch ( context.PipelineKey )
            {
                case StrategyKind.Retry:
                    builder.AddRetry(
                        new()
                        {
                            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                            Delay = TimeSpan.FromSeconds( 1 ),
                            BackoffType = DelayBackoffType.Exponential,
                            MaxRetryAttempts = 10
                        } );
                    ;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( strategyKind ) );
            }
        } );

    public ResiliencePipeline<T> GetPipeline<T>( StrategyKind strategyKind )
        => this._registry.GetOrAddPipeline<T>( strategyKind, ( builder, context ) =>
        {
            switch ( context.PipelineKey )
            {
                case StrategyKind.Retry:
                    builder.AddRetry(
                        new()
                        {
                            ShouldHandle = new PredicateBuilder<T>().Handle<Exception>(),
                            Delay = TimeSpan.FromSeconds( 1 ),
                            BackoffType = DelayBackoffType.Exponential,
                            MaxRetryAttempts = 10
                        } );
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( strategyKind ) );
            }
        } );

    public void Dispose()
    {
        this._registry.Dispose();
    }
}