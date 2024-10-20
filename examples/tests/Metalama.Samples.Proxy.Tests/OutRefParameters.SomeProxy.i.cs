using Metalama.Samples.Proxy.Tests.OutRefParameters;
namespace Metalama.Samples.Proxy.Tests
{
  public class SomeProxy : ISomeInterface
  {
    private ISomeInterface _intercepted;
    private IInterceptor _interceptor;
    public SomeProxy(IInterceptor interceptor, ISomeInterface intercepted)
    {
      _interceptor = interceptor;
      _intercepted = intercepted;
    }
    public int NonVoidMethod(out int a, ref string b, in DateTime dt, ref readonly TimeSpan ts)
    {
      var args = ((int, string, DateTime, TimeSpan))(default(int), b, dt, ts);
      try
      {
        return _interceptor!.Invoke(ref args, Invoke);
      }
      finally
      {
        a = args.Item1;
        b = args.Item2;
      }
      int Invoke(ref (int, string, DateTime, TimeSpan) receivedArgs)
      {
        return _intercepted.NonVoidMethod(out receivedArgs.Item1, ref receivedArgs.Item2, receivedArgs.Item3, receivedArgs.Item4);
      }
    }
    public void VoidMethod(out int a, ref string b, in DateTime dt, ref readonly TimeSpan ts)
    {
      var args = ((int, string, DateTime, TimeSpan))(default(int), b, dt, ts);
      try
      {
        _interceptor!.Invoke(ref args, Invoke);
      }
      finally
      {
        a = args.Item1;
        b = args.Item2;
      }
      ValueTuple Invoke(ref (int, string, DateTime, TimeSpan) receivedArgs)
      {
        _intercepted.VoidMethod(out receivedArgs.Item1, ref receivedArgs.Item2, receivedArgs.Item3, receivedArgs.Item4);
        return default;
      }
    }
  }
}