﻿using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8618, CS0649

public class RetryAttribute : OverrideMethodAspect
{
    [IntroduceDependency]
    private readonly ILogger _logger;

    [IntroduceDependency]
    private readonly IPolicyFactory _policyFactory;

    public PolicyKind Kind { get; }

    public RetryAttribute( PolicyKind kind = PolicyKind.Retry )
    {
        this.Kind = kind;
    }

    // Template for non-async methods.
    public override dynamic? OverrideMethod()
    {
        object? ExecuteCore()
        {
            try
            {
                return meta.Proceed();
            }
            catch ( Exception e )
            {
                var messageBuilder = LoggingHelper.BuildInterpolatedString( false );
                messageBuilder.AddText( " has failed: " );
                messageBuilder.AddExpression( e.Message );
                this._logger.LogWarning( (string) messageBuilder.ToValue() );

                throw;
            }
        }

        var policy = this._policyFactory.GetPolicy( this.Kind );
        return policy.Execute( ExecuteCore );
    }

    // Template for async methods.
    public override async Task<dynamic?> OverrideAsyncMethod()
    {
        async Task<object?> ExecuteCoreAsync( CancellationToken cancellationToken )
        {
            try
            {
                return await meta.ProceedAsync();
            }
            catch ( Exception e )
            {
                var messageBuilder = LoggingHelper.BuildInterpolatedString( false );
                messageBuilder.AddText( " has failed: " );
                messageBuilder.AddExpression( e.Message );
                this._logger.LogWarning( (string) messageBuilder.ToValue() );

                throw;
            }
        }

        var cancellationTokenParameter
            = meta.Target.Parameters.LastOrDefault( p => p.Type.Is( typeof(CancellationToken) ) );

        var policy = this._policyFactory.GetAsyncPolicy( this.Kind );
        return await policy.ExecuteAsync( ExecuteCoreAsync,
            cancellationTokenParameter != null
                ? (CancellationToken) cancellationTokenParameter.Value!
                : CancellationToken.None );
    }
}