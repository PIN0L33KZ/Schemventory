namespace Schemventory.App;

internal static class Helper
{
    public static void EnsureAppDataPath()
    {
        var appDataPath = GetAppDataPath();

        if(!Directory.Exists(appDataPath))
            _ = Directory.CreateDirectory(appDataPath);
    }

    public static string GetAppDataPath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.AppName);
    }
}