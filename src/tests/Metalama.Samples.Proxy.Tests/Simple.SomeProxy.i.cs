using System.Reflection;
using Metalama.Samples.Proxy.Tests.Simple;
namespace Metalama.Samples.Proxy.Tests
{
  public class SomeProxy : ISomeInterface
  {
    private ISomeInterface _intercepted;
    private IInterceptor _interceptor;
    private static InterceptionMetadata _metadata1;
    private static InterceptionMetadata _metadata2;
    private static InterceptionMetadata _metadata3;
    static SomeProxy()
    {
      _metadata1 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("VoidMethod", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string) }, null) ?? throw new MissingMethodException("The method 'ISomeInterface.VoidMethod(int, string)' could not be found using reflection."), false);
      _metadata2 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("NonVoidMethod", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string) }, null) ?? throw new MissingMethodException("The method 'ISomeInterface.NonVoidMethod(int, string)' could not be found using reflection."), false);
      _metadata3 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("VoidNoParamMethod", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) ?? throw new MissingMethodException("The method 'ISomeInterface.VoidNoParamMethod()' could not be found using reflection."), false);
    }
    public SomeProxy(IInterceptor interceptor, ISomeInterface intercepted)
    {
      _interceptor = interceptor;
      _intercepted = intercepted;
    }
    public int NonVoidMethod(int a, string b)
    {
      var args = (a, b);
      return _interceptor.Invoke(ref args, _metadata2, Invoke);
      int Invoke(ref (int a, string b) receivedArgs)
      {
        return _intercepted.NonVoidMethod(receivedArgs.a, receivedArgs.b);
      }
    }
    public void VoidMethod(int a, string b)
    {
      var args = (a, b);
      _interceptor.Invoke(ref args, _metadata1, Invoke);
      return;
      ValueTuple Invoke(ref (int a, string b) receivedArgs)
      {
        _intercepted.VoidMethod(receivedArgs.a, receivedArgs.b);
        return default;
      }
    }
    public void VoidNoParamMethod()
    {
      var args = ValueTuple.Create();
      _interceptor.Invoke(ref args, _metadata3, Invoke);
      return;
      ValueTuple Invoke(ref ValueTuple receivedArgs)
      {
        _intercepted.VoidNoParamMethod();
        return default;
      }
    }
  }
}