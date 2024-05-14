using System.Collections.Concurrent;

public partial class PerformanceCounterManager
{
    public void PrintAndReset()
    {
        var oldCounters = this._counters;
        var elapsedMilliseconds = this._stopwatch.ElapsedMilliseconds;
        
        this._counters = new ConcurrentDictionary<string, int>();
        this._stopwatch.Reset();

        foreach ( var counter in oldCounters )
        {
            Console.WriteLine($"{counter.Key}: {1000d * counter.Value/elapsedMilliseconds} calls/s");
        }

    }
}