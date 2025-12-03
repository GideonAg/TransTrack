using Logging;
using System.Text;

namespace TransTrack.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private static readonly object _lockObject = new object();

        public FileLogger(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                throw new ArgumentException("Log file path cannot be null or empty", nameof(logFilePath));
            }

            _logFilePath = logFilePath;
            EnsureLogDirectoryExists();
        }

        public FileLogger() : this(@"C:\TransTrack\Logs\system.log")
        {
        }

        private void EnsureLogDirectoryExists()
        {
            try
            {
                string directory = Path.GetDirectoryName(_logFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create log directory: {ex.Message}", ex);
            }
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARN", message);
        }

        public void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        public void LogError(string message, Exception ex)
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine(message);
            errorMessage.AppendLine($"Exception: {ex.GetType().Name}");
            errorMessage.AppendLine($"Message: {ex.Message}");
            errorMessage.AppendLine($"StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                errorMessage.AppendLine($"Inner Exception: {ex.InnerException.Message}");
            }

            WriteLog("ERROR", errorMessage.ToString());
        }

        public void LogDebug(string message)
        {
            WriteLog("DEBUG", message);
        }

        private void WriteLog(string level, string message)
        {
            try
            {
                lock (_lockObject)
                {
                    string logEntry = FormatLogEntry(level, message);
                    File.AppendAllText(_logFilePath, logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOGGING ERROR: {ex.Message}");
                Console.WriteLine($"Original message: [{level}] {message}");
            }
        }

        private string FormatLogEntry(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var threadId = Thread.CurrentThread.ManagedThreadId;
            return $"[{timestamp}] [{level}] [Thread-{threadId}] {message}{Environment.NewLine}";
        }

        public string LogFilePath => _logFilePath;
    }
}