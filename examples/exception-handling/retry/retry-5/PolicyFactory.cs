using Polly;
using Polly.Retry;

internal class PolicyFactory : IPolicyFactory
{
    private static readonly RetryPolicy _retry = Policy.Handle<Exception>().WaitAndRetry(
        new[]
        {
            TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 2 ), TimeSpan.FromSeconds( 4 ),
            TimeSpan.FromSeconds( 8 ), TimeSpan.FromSeconds( 15 ), TimeSpan.FromSeconds( 30 )
        } );

    private static readonly AsyncRetryPolicy _asyncRetry = Policy.Handle<Exception>()
        .WaitAndRetryAsync(
            new[]
            {
                TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 2 ), TimeSpan.FromSeconds( 4 ),
                TimeSpan.FromSeconds( 8 ), TimeSpan.FromSeconds( 15 ),
                TimeSpan.FromSeconds( 30 )
            } );


    public Policy GetPolicy( PolicyKind policyKind )
        => policyKind switch
        {
            PolicyKind.Retry => _retry,
            _ => throw new ArgumentOutOfRangeException( nameof(policyKind) )
        };

    public AsyncPolicy GetAsyncPolicy( PolicyKind policyKind )
        => policyKind switch
        {
            PolicyKind.Retry => _asyncRetry,
            _ => throw new ArgumentOutOfRangeException( nameof(policyKind) )
        };
}