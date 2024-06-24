using ThreadLoggerDemo;

internal class Program
{
    static RunThreads? _runThreads;

    static void Main(string[] args)
    {
        // hook up an application level handler for unhandled application
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

        // create an instance of the RunThreads class and execute the demo
        _runThreads = new RunThreads();
        _runThreads.RunDemo();

        // Wait for a key press before exiting
        Console.WriteLine($"{Environment.NewLine}Press any key to exit.");
        Console.Read();
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
