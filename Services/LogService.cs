using System.IO;

namespace BOOKpandoc.Services
{
    public class LogService
    {
        private readonly string _logFilePath;

        public LogService()
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            _logFilePath = Path.Combine(logDirectory, $"BOOKpandoc_{DateTime.Now:yyyyMMdd}.log");
        }

        public void Info(string message)
        {
            WriteLog("INFO", message);
        }

        public void Warning(string message)
        {
            WriteLog("WARNING", message);
        }

        public void Error(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                message += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
            }
            WriteLog("ERROR", message);
        }

        private void WriteLog(string level, string message)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var separator = new string('=', 80);
                var logEntry = $"{separator}\n[{timestamp}] 【{level}】\n{message}\n{separator}";
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine + Environment.NewLine, System.Text.Encoding.UTF8);
            }
            catch
            {
                // 忽略日志写入错误
            }
        }
    }
}