using System.Runtime.CompilerServices;

namespace Metalama.Samples.Proxy;

public interface IInterceptor
{
    public TResult Invoke<TArgs, TResult>( ref TArgs args, InterceptorDelegate<TArgs, TResult> proceed) where TArgs : struct, ITuple;

    public Task<TResult> InvokeAsync<TArgs, TResult>( ref TArgs args, AsyncInterceptorDelegate<TArgs, TResult> proceed) where TArgs : struct, ITuple;
}

public delegate TResult InterceptorDelegate<TArgs, out TResult>(ref TArgs args)
    where TArgs : struct, ITuple;

public delegate Task<TResult> AsyncInterceptorDelegate<TArgs, TResult>(in TArgs args ) where TArgs : struct, ITuple;
