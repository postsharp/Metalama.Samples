using System.Reflection;
using Metalama.Samples.Proxy.Tests.OutRefParameters;

namespace Metalama.Samples.Proxy.Tests
{
    public class SomeProxy : ISomeInterface
    {
        private ISomeInterface _intercepted;
        private IInterceptor _interceptor;
        private static InterceptionMetadata _metadata1;
        private static InterceptionMetadata _metadata2;

        static SomeProxy()
        {
      _metadata1 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("VoidMethod", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int).MakeByRefType(), typeof(string).MakeByRefType(), typeof(DateTime).MakeByRefType(), typeof(TimeSpan).MakeByRefType() }, null), false);
      _metadata2 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("NonVoidMethod", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int).MakeByRefType(), typeof(string).MakeByRefType(), typeof(DateTime).MakeByRefType(), typeof(TimeSpan).MakeByRefType() }, null), false);
        }

        public SomeProxy(IInterceptor interceptor, ISomeInterface intercepted)
        {
            _interceptor = interceptor;
            _intercepted = intercepted;
        }

        public int NonVoidMethod(out int a, ref string b, in DateTime dt, ref readonly TimeSpan ts)
        {
      var args = (a: default(int), b, dt, ts);
            try
            {
                return _interceptor.Invoke(ref args, _metadata2, Invoke);
            }
            finally
            {
        a = args.a;
        b = args.b;
            }
      int Invoke(ref (int a, string b, DateTime dt, TimeSpan ts) receivedArgs)
            {
        return _intercepted.NonVoidMethod(out receivedArgs.a, ref receivedArgs.b, receivedArgs.dt, receivedArgs.ts);
            }
        }

        public void VoidMethod(out int a, ref string b, in DateTime dt, ref readonly TimeSpan ts)
        {
      var args = (a: default(int), b, dt, ts);
            try
            {
                _interceptor.Invoke(ref args, _metadata1, Invoke);
            }
            finally
            {
        a = args.a;
        b = args.b;
            }

            return;
      ValueTuple Invoke(ref (int a, string b, DateTime dt, TimeSpan ts) receivedArgs)
            {
        _intercepted.VoidMethod(out receivedArgs.a, ref receivedArgs.b, receivedArgs.dt, receivedArgs.ts);
                return default;
            }
        }
    }
}