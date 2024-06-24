using ThreadLoggerDemo;

internal class Program
{
    static RunThreads? _runThreads;

    static void Main(string[] args)
    {
        // hook up an application level handler for unhandled application
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

        // Validate that there is a command line parameter.
        if (args.Length == 0)
        {
            Console.WriteLine("ThreadLoggerDemo requires a command line parameter specifying the log file location (path).");
        }

        var logfilePath = args[0];
        if (ValidatePath(logfilePath))
        {
            // create an instance of the RunThreads class and execute the demo
            _runThreads = new RunThreads(logfilePath);
            _runThreads.RunDemo();
        }

        // Wait for a key press before exiting
        Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
        Console.ReadKey();
    }

    /// <summary>
    /// Validates that the path in the command line:
    /// b) it validates that its a valid directory path  AND the path does not exist, it will create the path.
    /// </summary>
    static bool ValidatePath(string logfilePath)
    {
        if (!string.IsNullOrEmpty(logfilePath))
        {
            // will create directory if it does not exist, will throw exception if the path is not valid
            // If directory exist, it just returns the directoryinfo object which is not used here.

            var fullPath = Path.Combine(logfilePath, "log");
            try
            {
                Directory.CreateDirectory(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid command line path for log file ({fullPath}){Environment.NewLine}{ex.Message}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Event handler for all unhandled exception including any that are thrown from a running thread that would be disconnected from the UI
    /// When the handler is triggered, it will notify the user with the expection and its details 
    /// and then calls RunThreads.StopAllThreads to stop all running threads before shutting down the application
    /// </summary>
    static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Handle the exception (log it, show a message to the user, etc.)
        Exception ex = (Exception)e.ExceptionObject;
        Console.WriteLine($"An unhandled exception occurred: {ex.Message}{Environment.NewLine}Stopping all running threads and exiting application.");

        if (_runThreads != null)
        {
            _runThreads.StopAllThreads();
        }

        // Wait for a key press before exiting
        Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
        Console.ReadKey();

        Environment.Exit(1);
    }
}
