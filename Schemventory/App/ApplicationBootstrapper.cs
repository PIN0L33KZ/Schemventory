using Serilog;
using Schemventory.Services;

namespace Schemventory.App;

internal static class ApplicationBootstrapper
{
    private const string LogContext = "(Subsystem)";
    internal static void Run()
    {
        ConfigureWinForms();
        RegisterGlobalExceptionHandlers();

        DatabaseService databaseService = new();
        ItemIconService itemIconService = new();
        ItemDataProvider itemDataProvider = new();

        Log.Debug("{LogContext} Initialising database.", LogContext);
        databaseService.InitializeDatabase();

        Log.Debug("{LogContext} Launching recent projects window.", LogContext);
        Application.Run(new FRM_RecentProjects(databaseService, itemIconService, itemDataProvider));
    }

    private static void ConfigureWinForms()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        Log.Debug("{LogContext} WinForms initialised.", LogContext);
    }

    private static void RegisterGlobalExceptionHandlers()
    {
        Application.ThreadException += OnApplicationThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;

        Log.Debug("{LogContext} Global exception handlers registered.", LogContext);
    }

    private static void OnApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        Log.Error(e.Exception, "{LogContext} Unhandled UI thread exception.", LogContext);

        _ = MessageBox.Show("An unexpected error occurred and the application needs to close.", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

        Application.Exit();
    }

    private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if(e.ExceptionObject is Exception exception)
            Log.Fatal(exception, "{LogContext} Unhandled non-UI exception.", LogContext);
        else
            Log.Fatal("{LogContext} Unhandled non-UI exception (non-Exception object).", LogContext);
    }
}
