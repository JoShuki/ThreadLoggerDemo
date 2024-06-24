namespace ThreadLoggerDemo
{
    /// <summary>
    /// A Thread Safe File logger
    /// </summary>
    internal class ThreadSafeLogger
    {
        private string _logFilePath = string.Empty;
        private readonly object _fileLock = new object();
        private int _lineCount = 0;


        /// <summary>
        /// </summary>
        /// <param name="initialEntry">Initial entry that is written when the log file is created</param>
        /// <param name="logfilePath">Path to the log file</param>
        internal ThreadSafeLogger(string logfilePath, string initialEntry)
        {
            // save the filepath and write the initial entry to the log file.
            _logFilePath = logfilePath;
            Console.WriteLine($"Creating log file with initial entry at {Path.GetFullPath(logfilePath)}");
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
