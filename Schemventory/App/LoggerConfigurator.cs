using Serilog;
using Serilog.Events;

namespace Schemventory.App;

internal static class LoggerConfigurator
{
    private const string LogContext = "(Subsystem)";

    internal static void Configure()
    {
        var logDirectory = Constants.AppLogDir;

        try
        {
            _ = Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(path: Path.Combine(logDirectory, Constants.LogFilePattern), outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, retainedFileCountLimit: Constants.MaxLogFiles, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(Constants.LogFlushIntervalSeconds))
                .CreateLogger();

            Log.Verbose("________________________________________________________________________________________________");
            Log.Debug("{LogContext} Log directory: {LogDirectory}", LogContext, logDirectory);
        }
        catch(Exception ex)
        {
            _ = MessageBox.Show($"Failed to initialise logging.\n\n{ex.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
    }

    internal static string GetLogDirectory()
    {
        return Constants.AppLogDir;
    }
}