using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using WhimsyLog.Models;

namespace WhimsyLog
{
    // adapted from https://stackoverflow.com/questions/40073743/how-to-log-to-a-file-without-using-third-party-logger-in-net-core#answer-68363461
    public class QueueLogger : ILogger
    {
        private ConcurrentQueue<string> _concurrentQueue;
        public QueueLogger(ConcurrentQueue<string> concurrentQueue)
        {
            _concurrentQueue = concurrentQueue;
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
                var now = DateTime.Now;
                var n = Environment.NewLine;
                string exc = "";
                if (exception != null) exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                string message = logLevel.ToString() + ": " + now.ToString() + " " + formatter(state, exception) + n + exc;
                _concurrentQueue.Enqueue(message);
            }
        }
    }
}
