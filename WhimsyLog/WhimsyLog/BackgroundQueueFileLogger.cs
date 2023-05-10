using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using WhimsyLog.Models;

namespace WhimsyLog
{
    public class BackgroundQueueFileLogger : BackgroundService
    {
        private readonly string _folderPath;
        private readonly ConcurrentQueue<string> _concurrentQueue;
        private readonly int _delayMilliseconds;
        public BackgroundQueueFileLogger(string folderPath, ConcurrentQueue<string> concurrentQueue, int delayMilliseconds)
        {   
            _folderPath = folderPath;
            _concurrentQueue = concurrentQueue;
            _delayMilliseconds = delayMilliseconds;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(_delayMilliseconds), stoppingToken);

                var messagesToWrite = new List<string>();
                while (_concurrentQueue.TryDequeue(out var message))
                {
                    messagesToWrite.Add(message);
                }

                if (messagesToWrite.Count > 0)
                {
                    await WriteMessagesToFile(messagesToWrite);
                }
            }
        }

        private async Task WriteMessagesToFile(List<string> messages)
        {
            string fullFilePath = Path.Combine(_folderPath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            string combinedMessage = string.Join(string.Empty, messages);

            await File.AppendAllTextAsync(fullFilePath, combinedMessage);
        }
    }
}
