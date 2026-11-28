namespace Schemventory.App;

internal static class Constants
{
    // Publisher
    internal const string Publisher = "PIN0L33KZ GbR";
    internal const string PublisherUrl = "https://pinoleekz.de/";
    internal static string Copyright => $"© {Publisher}, {DateTime.Now.Year}";

    // Application
    internal const string AppName = "Schemventory";

    // Directories
    internal static readonly string AppDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Publisher, AppName);
    internal static readonly string AppCacheDir = Path.Combine(AppDir, "Cache");
    internal static readonly string AppLogDir = Path.Combine(AppDir, "logs");
    internal static readonly string AppDatabasePath = Path.Combine(AppDir, "Schemventory.db");

    // Logging
    internal const string LogFilePattern = "log-.txt";
    internal const int MaxLogFiles = 10;
    internal const int LogFlushIntervalSeconds = 2;

    // Material Colours
    internal static readonly Color MissingColor = Color.FromArgb(217, 74, 74);
    internal static readonly Color CollectedColor = Color.FromArgb(71, 213, 166);
    internal static readonly Color IgnoredColor = Color.FromArgb(215, 172, 97);
    internal static readonly Color ReplacedColor = Color.FromArgb(43, 133, 187);

    // Material Controls
    internal static readonly Color DefaultColor = Color.FromArgb(58, 55, 55);
    internal static readonly Color DefaultBorderColor = Color.FromArgb(103, 99, 99);

    // Material Drag and Drop
    internal const int MaterialDragStartDistancePixels = 8;
    internal const int MaterialDragUpdateIntervalMilliseconds = 16;
    internal const int MaterialDragAutoScrollMarginPixels = 36;
    internal const int MaterialDragAutoScrollStepPixels = 12;
    internal const double MaterialDragPreviewOpacity = 0.25;
    internal static readonly Color MaterialDragPlaceholderColor = Color.FromArgb(162, 123, 90);
    internal static readonly Color MaterialDragPlaceholderBackgroundColor = Color.FromArgb(25, 194, 165, 142);
}