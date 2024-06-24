namespace ThreadLoggerDemo
{
    /// <summary>
    /// A Thread Safe File logger
    /// </summary>
    internal class ThreadSafeLogger
    {
        private const string LOG_FILENAME = "out.txt";
        private string _logFilePath = "log";
        private readonly object _fileLock = new object();
        private int _lineCount = 0;


        /// <summary>
        /// This method creates log file and writes an initial entry to it 
        /// </summary>
        /// <param name="logfilePath">Path to the log file</param>
        /// <param name="initialEntry">Initial entry to log file</param>
        internal void CreateLogFile(string logfilePath, string initialEntry)
        {
            _logFilePath = Path.Combine(logfilePath, _logFilePath, LOG_FILENAME);
            File.WriteAllText(_logFilePath, $"{_lineCount}, {initialEntry}, {DateTime.Now:HH:mm:ss.fff}{Environment.NewLine}");
        }

        /// <summary>
        /// Appends a new entry to the logfile that is preceeded with the sequential entry number and followed with a time stamp and new line delimeter.
        /// </summary>
        /// <param name="logEntry">Text of log file to append</param>
        internal void AddLogEntry(string logEntry)
        {
            lock (_fileLock)
            {
                var lineNumber = ++_lineCount;
                File.AppendAllText(_logFilePath, $"{lineNumber}, {logEntry}, {DateTime.Now:HH:mm:ss.fff}{Environment.NewLine}");
            }
        }
    }
}
