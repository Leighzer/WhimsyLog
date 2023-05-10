using Microsoft.Extensions.Logging;

namespace WhimsyLog
{
    // adapted from https://stackoverflow.com/questions/40073743/how-to-log-to-a-file-without-using-third-party-logger-in-net-core#answer-68363461
    public class FileLogger : ILogger
    {
        private string _folderPath;
        private static object _lock = new object();
        public FileLogger(string path)
        {
            _folderPath = path;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {   
            if (formatter != null)
            {
                lock (_lock)
                {
                    string fullFilePath = Path.Combine(_folderPath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                    var n = Environment.NewLine;
                    string exc = "";
                    if (exception != null) exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                    File.AppendAllText(fullFilePath, logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
                }
            }
        }
    }
}
