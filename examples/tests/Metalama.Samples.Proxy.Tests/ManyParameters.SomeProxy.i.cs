using System.Reflection;
using Metalama.Samples.Proxy.Tests.ManyParameters;

namespace Metalama.Samples.Proxy.Tests
{
    public class SomeProxy : ISomeInterface
    {
        private ISomeInterface _intercepted;
        private IInterceptor _interceptor;
        private static InterceptionMetadata _metadata1;
        private static InterceptionMetadata _metadata2;
        private static InterceptionMetadata _metadata3;
        private static InterceptionMetadata _metadata4;

        static SomeProxy()
        {
      _metadata1 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("Void10Params", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string), typeof(long), typeof(DateOnly), typeof(TimeSpan), typeof(Guid), typeof(short), typeof(int? ), typeof(TimeZoneInfo), typeof(object) }, null), false);
      _metadata2 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("NonVoid10Params", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string), typeof(long), typeof(DateOnly), typeof(TimeSpan), typeof(Guid), typeof(short), typeof(int? ), typeof(TimeZoneInfo), typeof(object) }, null), false);
      _metadata3 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("Void20Params", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string), typeof(long), typeof(DateOnly), typeof(TimeSpan), typeof(Guid), typeof(short), typeof(int? ), typeof(TimeZoneInfo), typeof(object), typeof(string), typeof(string), typeof(DateTime), typeof(IDisposable), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(long? ), typeof(object), typeof(object) }, null), false);
      _metadata4 = new InterceptionMetadata(typeof(ISomeInterface).GetMethod("NonVoid20Params", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string), typeof(long), typeof(DateOnly), typeof(TimeSpan), typeof(Guid), typeof(short), typeof(int? ), typeof(TimeZoneInfo), typeof(object), typeof(string), typeof(string), typeof(DateTime), typeof(IDisposable), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(long? ), typeof(object), typeof(object) }, null), false);
        }

        public SomeProxy(IInterceptor interceptor, ISomeInterface intercepted)
        {
            _interceptor = interceptor;
            _intercepted = intercepted;
        }
    public void NonVoid10Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10)
        {
            var args = (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
            _interceptor.Invoke(ref args, _metadata2, Invoke);
            return;
      ValueTuple Invoke(ref (int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10) receivedArgs)
      {
        _intercepted.NonVoid10Params(receivedArgs.p1, receivedArgs.p2, receivedArgs.p3, receivedArgs.p4, receivedArgs.p5, receivedArgs.p6, receivedArgs.p7, receivedArgs.p8, receivedArgs.p9, receivedArgs.p10);
                return default;
            }
        }
    public long NonVoid20Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10, string? p11, string p12, DateTime p13, IDisposable p14, DateTime p15, DateTime p16, DateTime p17, long? p18, object p19, object p20)
        {
      var args = (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20);
            return _interceptor.Invoke(ref args, _metadata4, Invoke);
      long Invoke(ref (int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10, string? p11, string p12, DateTime p13, IDisposable p14, DateTime p15, DateTime p16, DateTime p17, long? p18, object p19, object p20) receivedArgs)
      {
        return _intercepted.NonVoid20Params(receivedArgs.p1, receivedArgs.p2, receivedArgs.p3, receivedArgs.p4, receivedArgs.p5, receivedArgs.p6, receivedArgs.p7, receivedArgs.p8, receivedArgs.p9, receivedArgs.p10, receivedArgs.p11, receivedArgs.p12, receivedArgs.p13, receivedArgs.p14, receivedArgs.p15, receivedArgs.p16, receivedArgs.p17, receivedArgs.p18, receivedArgs.p19, receivedArgs.p20);
      }
    }
    public void Void10Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10)
        {
            var args = (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
            _interceptor.Invoke(ref args, _metadata1, Invoke);
            return;
      ValueTuple Invoke(ref (int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10) receivedArgs)
      {
        _intercepted.Void10Params(receivedArgs.p1, receivedArgs.p2, receivedArgs.p3, receivedArgs.p4, receivedArgs.p5, receivedArgs.p6, receivedArgs.p7, receivedArgs.p8, receivedArgs.p9, receivedArgs.p10);
                return default;
            }
        }
    public void Void20Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10, string? p11, string p12, DateTime p13, IDisposable p14, DateTime p15, DateTime p16, DateTime p17, long? p18, object p19, object p20)
        {
      var args = (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20);
            _interceptor.Invoke(ref args, _metadata3, Invoke);
            return;
      ValueTuple Invoke(ref (int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10, string? p11, string p12, DateTime p13, IDisposable p14, DateTime p15, DateTime p16, DateTime p17, long? p18, object p19, object p20) receivedArgs)
      {
        _intercepted.Void20Params(receivedArgs.p1, receivedArgs.p2, receivedArgs.p3, receivedArgs.p4, receivedArgs.p5, receivedArgs.p6, receivedArgs.p7, receivedArgs.p8, receivedArgs.p9, receivedArgs.p10, receivedArgs.p11, receivedArgs.p12, receivedArgs.p13, receivedArgs.p14, receivedArgs.p15, receivedArgs.p16, receivedArgs.p17, receivedArgs.p18, receivedArgs.p19, receivedArgs.p20);
                return default;
            }
        }
    }
}