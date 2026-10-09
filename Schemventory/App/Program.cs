using Serilog;
using System.Reflection;

namespace Schemventory.App;

internal static class Program
{
    private const string LogContext = "(Subsystem)";
    [STAThread]
    private static void Main()
    {
        try
        {
            LoggerConfigurator.Configure();

            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

            Log.Information("{LogContext} Application starting. (Version {Version})", LogContext, version);

            ApplicationBootstrapper.Run();

            Log.Information("{LogContext} Application closed normally.", LogContext);
        }
        catch(Exception ex)
        {
            Log.Fatal(ex, "{LogContext} Fatal startup or runtime error.", LogContext);

            _ = MessageBox.Show($"A fatal error occurred.\n\n{ex.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
