using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace WhimsyLog
{
    public class QueueLoggerProvider : ILoggerProvider
    {
        private ConcurrentQueue<string> _concurrentQueue;
        public QueueLoggerProvider(ConcurrentQueue<string> concurrentQueue)
        {
            _concurrentQueue = concurrentQueue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new QueueLogger(_concurrentQueue);
        }

        public void Dispose()
        {
        }
    }
}
