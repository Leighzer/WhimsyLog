using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace WhimsyLog
{
    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string folderPath)
        {
            builder.AddProvider(new FileLoggerProvider(folderPath));
            return builder;
        }

        public static ILoggingBuilder AddQueue(this ILoggingBuilder builder, ConcurrentQueue<string> concurrentQueue)
        {
            builder.AddProvider(new QueueLoggerProvider(concurrentQueue));
            return builder;
        }
    }
}
