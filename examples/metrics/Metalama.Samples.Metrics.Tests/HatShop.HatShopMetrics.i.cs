using System.Diagnostics.Metrics;
using System.Reflection;
using Metalama.Framework.RunTime;
using Metalama.Samples.Metrics;
using Metalama.Samples.Metrics.Example;
namespace Metrics
{
  public class HatShopMetrics
  {
    internal readonly Counter<long> PlaceOrderExceptionCount;
    internal readonly Counter<long> PlaceOrderExecutionCount;
    internal readonly Counter<long> PlaceOrderExecutionTime;
    private IMeterFactory _meterFactory;
    private IMetricHost _metricHost;
    public HatShopMetrics([AspectGenerated] IMeterFactory? meterFactory = null, [AspectGenerated] IMetricHost? metricHost = null)
    {
      this._meterFactory = meterFactory ?? throw new System.ArgumentNullException(nameof(meterFactory));
      this._metricHost = metricHost ?? throw new System.ArgumentNullException(nameof(metricHost));
      var meter = _meterFactory.Create(_metricHost.ApplicationName, _metricHost.ApplicationVersion, _metricHost.Tags);
      PlaceOrderExceptionCount = meter.CreateCounter<long>("PlaceOrder.ExceptionCount");
      _metricHost.RegisterInstrument(PlaceOrderExceptionCount, typeof(HatShop).GetMethod("PlaceOrder", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) ?? throw new MissingMethodException("The method 'HatShop.PlaceOrder()' could not be found using reflection."), "ExceptionCount");
      PlaceOrderExecutionCount = meter.CreateCounter<long>("PlaceOrder.ExecutionCount");
      _metricHost.RegisterInstrument(PlaceOrderExecutionCount, typeof(HatShop).GetMethod("PlaceOrder", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) ?? throw new MissingMethodException("The method 'HatShop.PlaceOrder()' could not be found using reflection."), "ExecutionCount");
      PlaceOrderExecutionTime = meter.CreateCounter<long>("PlaceOrder.ExecutionTime");
      _metricHost.RegisterInstrument(PlaceOrderExecutionTime, typeof(HatShop).GetMethod("PlaceOrder", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) ?? throw new MissingMethodException("The method 'HatShop.PlaceOrder()' could not be found using reflection."), "ExecutionTime");
    }
  }
}