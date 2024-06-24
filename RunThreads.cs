using System.Diagnostics;

namespace ThreadLoggerDemo
{
    /// <summary>
    /// The main class for the Thread Logger Demo
    /// </summary>
    internal class RunThreads
    {
        ThreadSafeLogger? _threadSafeLogger;

        const string LOGFILE_PATH = "/log/out.txt";
        Thread[] _threads = new Thread[10];
        bool _stopAllThreads = false;


        /// <summary>
        /// Constructor for class
        /// Creates the log directory if does not exist and writes the initial entry to the log.
        /// </summary>
        internal void RunDemo()
        {
            // initialize log file
            var initialEntry = $"0";
            try
            {
                var path = Path.GetDirectoryName(LOGFILE_PATH);
                Console.WriteLine($"Creating log directory at {path}");
                Directory.CreateDirectory(path);
                _threadSafeLogger = new ThreadSafeLogger(LOGFILE_PATH, initialEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot create log file({LOGFILE_PATH}){Environment.NewLine}{ex.Message}");
                return;
            }

            for (int i = 0; i < 10; i++)
            {
                _threads[i] = new Thread(new ThreadStart(LogEntriesThread));
                _threads[i].Name = $"LoggerThread {i}";
                _threads[i].Start();
            }

            var threads = string.Join(", ", _threads.Select(t => $"{t.ManagedThreadId} | {t.Name}"));
            Trace.WriteLine($"Running threads = {threads}");

            WaitForAllThreadsEnd();

            if (!_stopAllThreads)
            {
                Console.WriteLine("All threads successfully completed logging 10 entries and then terminated.");
            }
        }

        /// <summary>
        /// Stops all threads by setting the  _stopAllThreads flag to true and then waits for the logger threads to terminate
        /// </summary>
        internal void StopAllThreads()
        {
            _stopAllThreads = true;
            WaitForAllThreadsEnd();
        }

        /// <summary>
        /// Waits for the logger threads to terminate
        /// </summary>
        internal void WaitForAllThreadsEnd()
        {
            foreach (var thread in _threads)
            {
                var threads = string.Join(", ", _threads.Select(t => $"{t.ManagedThreadId} | {t.Name}"));
                if (thread != null && thread.IsAlive)
                {
                    thread.Join();
                }
            }
        }

        /// <summary>
        /// This is the method that is run by each of the logger threads.
        /// The method loops 10 times and each time appends the entry to the log file.
        /// The method calls the logger with the current thread id and the logger adds the entry counter and timestamp.
        /// </summary>
        private void LogEntriesThread()
        {
            for (int i = 0; i < 10; i++)
            {
                // stop the theard
                if (_stopAllThreads)
                    break;

                try
                {
                    var threadId = Environment.CurrentManagedThreadId;
                    _threadSafeLogger.AddLogEntry($"{threadId}");
                }
                catch (Exception ex)
                {
                    _stopAllThreads = true;
                    Console.WriteLine($"Exception occurred in LogEntriesThread, Thread Name:{Thread.CurrentThread.Name}, Thread ID: {Thread.CurrentThread.ManagedThreadId}" +
                        $"{Environment.NewLine}{ex.Message}");
                    break;
                }
            }
        }
    }
}
