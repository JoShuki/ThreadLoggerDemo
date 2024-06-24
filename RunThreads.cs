using System.Diagnostics;

namespace ThreadLoggerDemo
{
    internal class RunThreads
    {
        ThreadSafeLogger _threadSafeLogger = new ThreadSafeLogger();

        readonly string _logFilePath;
        Thread[] _threads = new Thread[10];
        bool _stopAllThreads = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logfilePath"></param>
        internal RunThreads(string logfilePath)
        {
            _logFilePath = logfilePath;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RunDemo()
        {
            // initialize log file
            var initialEntry = $"0";
            try
            {
                _threadSafeLogger.CreateLogFile(_logFilePath, initialEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot create log file({_logFilePath}){Environment.NewLine}{ex.Message}");
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
        /// 
        /// </summary>
        internal void StopAllThreads()
        {
            _stopAllThreads = true;
            WaitForAllThreadsEnd();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        private void LogEntriesThread()
        {
            //Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} -  STARTING Thread Name:{Thread.CurrentThread.Name}, Thread ID: {Thread.CurrentThread.ManagedThreadId}");

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

            //Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} -  ENDING Thread Name:{Thread.CurrentThread.Name}, Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
