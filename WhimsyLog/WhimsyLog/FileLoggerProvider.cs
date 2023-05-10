using Microsoft.Extensions.Logging;

namespace WhimsyLog
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private string _folderPath;
        public FileLoggerProvider(string folderPath)
        {
            _folderPath = folderPath;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_folderPath);
        }

        public void Dispose()
        {
        }
    }
}
