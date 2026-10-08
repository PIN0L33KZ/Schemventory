using Schemventory.Services;

namespace Schemventory.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        DatabaseService databaseService = new();
        ItemIconService itemIconService = new();
        ItemDataProvider itemDataProvider = new();

        databaseService.InitializeDatabase();

        Application.Run(new FRM_RecentProjects(
            databaseService,
            itemIconService,
            itemDataProvider));
    }
}
