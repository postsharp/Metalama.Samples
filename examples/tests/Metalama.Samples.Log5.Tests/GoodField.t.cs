using Microsoft.Extensions.Logging;
namespace Metalama.Samples.Log4.Tests.GoodField;
// ReSharper disable once NotAccessedField.Local
internal class Foo
{
  private readonly ILogger _logger;
  public Foo(ILogger logger)
  {
    this._logger = logger;
  }
  [Log]
  public void Method()
  {
    var logger = this._logger;
    var isTracingEnabled = logger.IsEnabled(LogLevel.Trace);
    if (isTracingEnabled)
    {
      LoggerExtensions.LogTrace(logger, $"Foo.Method() started.");
    }
    try
    {
      object result = null;
      if (isTracingEnabled)
      {
        LoggerExtensions.LogTrace(logger, $"Foo.Method() succeeded.");
      }
      return;
    }
    catch (Exception e)when (logger.IsEnabled(LogLevel.Warning))
    {
      LoggerExtensions.LogWarning(logger, $"Foo.Method() failed: {e.Message}");
      throw;
    }
  }
}