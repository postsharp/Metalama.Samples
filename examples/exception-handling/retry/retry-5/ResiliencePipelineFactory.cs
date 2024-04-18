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
                            DelayGenerator = static args => new ValueTask<TimeSpan?>(
                                args.AttemptNumber switch
                                {
                                    0 => TimeSpan.FromSeconds( 1 ),
                                    1 => TimeSpan.FromSeconds( 2 ),
                                    2 => TimeSpan.FromSeconds( 4 ),
                                    3 => TimeSpan.FromSeconds( 8 ),
                                    4 => TimeSpan.FromSeconds( 15 ),
                                    _ => TimeSpan.FromSeconds( 30 )
                                } ),
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
                            DelayGenerator = static args => new ValueTask<TimeSpan?>(
                                args.AttemptNumber switch
                                {
                                    0 => TimeSpan.FromSeconds( 1 ),
                                    1 => TimeSpan.FromSeconds( 2 ),
                                    2 => TimeSpan.FromSeconds( 4 ),
                                    3 => TimeSpan.FromSeconds( 8 ),
                                    4 => TimeSpan.FromSeconds( 15 ),
                                    _ => TimeSpan.FromSeconds( 30 )
                                } ),
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