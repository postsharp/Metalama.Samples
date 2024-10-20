using Metalama.Samples.Proxy.Tests.ManyParameters;
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
    public void NonVoid10Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10)
    {
      var args = ((int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object))(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
      _interceptor!.Invoke(ref args, Invoke);
      ValueTuple Invoke(ref (int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object) receivedArgs)
      {
        _intercepted.NonVoid10Params(receivedArgs.Item1, receivedArgs.Item2, receivedArgs.Item3, receivedArgs.Item4, receivedArgs.Item5, receivedArgs.Item6, receivedArgs.Item7, receivedArgs.Rest.Item1, receivedArgs.Rest.Item2, receivedArgs.Rest.Item3);
        return default;
      }
    }
    public long NonVoid20Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10, string? p11, string p12, DateTime p13, IDisposable p14, DateTime p15, DateTime p16, DateTime p17, long? p18, object p19, object p20)
    {
      var args = ((int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object, string? , string, DateTime, IDisposable, DateTime, DateTime, DateTime, long? , object, object))(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20);
      return _interceptor!.Invoke(ref args, Invoke);
      long Invoke(ref (int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object, string? , string, DateTime, IDisposable, DateTime, DateTime, DateTime, long? , object, object) receivedArgs)
      {
        return _intercepted.NonVoid20Params(receivedArgs.Item1, receivedArgs.Item2, receivedArgs.Item3, receivedArgs.Item4, receivedArgs.Item5, receivedArgs.Item6, receivedArgs.Item7, receivedArgs.Rest.Item1, receivedArgs.Rest.Item2, receivedArgs.Rest.Item3, receivedArgs.Rest.Item4, receivedArgs.Rest.Item5, receivedArgs.Rest.Item6, receivedArgs.Rest.Item7, receivedArgs.Rest.Rest.Item1, receivedArgs.Rest.Rest.Item2, receivedArgs.Rest.Rest.Item3, receivedArgs.Rest.Rest.Item4, receivedArgs.Rest.Rest.Item5, receivedArgs.Rest.Rest.Item6);
      }
    }
    public void Void10Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10)
    {
      var args = ((int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object))(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
      _interceptor!.Invoke(ref args, Invoke);
      ValueTuple Invoke(ref (int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object) receivedArgs)
      {
        _intercepted.Void10Params(receivedArgs.Item1, receivedArgs.Item2, receivedArgs.Item3, receivedArgs.Item4, receivedArgs.Item5, receivedArgs.Item6, receivedArgs.Item7, receivedArgs.Rest.Item1, receivedArgs.Rest.Item2, receivedArgs.Rest.Item3);
        return default;
      }
    }
    public void Void20Params(int p1, string p2, long p3, DateOnly p4, TimeSpan p5, Guid p6, short p7, int? p8, TimeZoneInfo? p9, object p10, string? p11, string p12, DateTime p13, IDisposable p14, DateTime p15, DateTime p16, DateTime p17, long? p18, object p19, object p20)
    {
      var args = ((int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object, string? , string, DateTime, IDisposable, DateTime, DateTime, DateTime, long? , object, object))(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20);
      _interceptor!.Invoke(ref args, Invoke);
      ValueTuple Invoke(ref (int, string, long, DateOnly, TimeSpan, Guid, short, int? , TimeZoneInfo? , object, string? , string, DateTime, IDisposable, DateTime, DateTime, DateTime, long? , object, object) receivedArgs)
      {
        _intercepted.Void20Params(receivedArgs.Item1, receivedArgs.Item2, receivedArgs.Item3, receivedArgs.Item4, receivedArgs.Item5, receivedArgs.Item6, receivedArgs.Item7, receivedArgs.Rest.Item1, receivedArgs.Rest.Item2, receivedArgs.Rest.Item3, receivedArgs.Rest.Item4, receivedArgs.Rest.Item5, receivedArgs.Rest.Item6, receivedArgs.Rest.Item7, receivedArgs.Rest.Rest.Item1, receivedArgs.Rest.Rest.Item2, receivedArgs.Rest.Rest.Item3, receivedArgs.Rest.Rest.Item4, receivedArgs.Rest.Rest.Item5, receivedArgs.Rest.Rest.Item6);
        return default;
      }
    }
  }
}