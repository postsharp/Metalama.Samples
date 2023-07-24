using Polly;

public interface IPolicyFactory
{
    Policy GetPolicy( PolicyKind policyKind );
    AsyncPolicy GetAsyncPolicy( PolicyKind policyKind );
}