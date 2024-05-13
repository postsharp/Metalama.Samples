using System.Collections.Concurrent;
using System.Diagnostics;

[Singleton]
public class PerformanceCounterManager( IPerformanceCounterUploader uploader )
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private ConcurrentDictionary<string, int> _counters = new();
    
    public void IncrementCounter( string name ) 
        => this._counters.AddOrUpdate( name, 1, ( _, value ) => value + 1 );
    
    public void UploadAndReset()
    {
        var oldCounters = this._counters;
        var elapsedMilliseconds = this._stopwatch.ElapsedMilliseconds;
        
        this._counters = new ConcurrentDictionary<string, int>();
        this._stopwatch.Reset();

        foreach ( var counter in oldCounters )
        {
            uploader.UploadCounter( counter.Key,  1000d * counter.Value/elapsedMilliseconds);
        }
    }
}