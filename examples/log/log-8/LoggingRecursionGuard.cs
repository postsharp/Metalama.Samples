internal static class LoggingRecursionGuard
{
    [ThreadStatic]
    public static bool _isLogging;

    public static DisposeCookie Begin()
    {
        if ( _isLogging )
        {
            return new DisposeCookie( false );
        }
        else
        {
            _isLogging = true;
            return new DisposeCookie( true );
        }
    }

    internal class DisposeCookie : IDisposable
    {
        public DisposeCookie( bool canLog )
        {
            this.CanLog = canLog;
        }

        public bool CanLog { get; }
        public void Dispose()
        {
            if ( this.CanLog )
            {
                _isLogging = false;
            }
        }
    }
}
