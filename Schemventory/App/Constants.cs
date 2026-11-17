namespace Schemventory.App;

internal class Constants
{
    public const string AppName = "Schemventory";
    public const string AppAuthor = "PIN0L33KZ GbR";
    public const string AppWebsite = "https://pinoleekz.de/";

    public const int MaterialDragStartDistancePixels = 8;
    public const int MaterialDragUpdateIntervalMilliseconds = 16;
    public const int MaterialDragAutoScrollMarginPixels = 36;
    public const int MaterialDragAutoScrollStepPixels = 12;
    public const double MaterialDragPreviewOpacity = 0.25;

    public static readonly string CacheDirectory = Path.Combine(Helper.GetAppDataPath(), "Cache");
    public static readonly string LogDirectory = Path.Combine(Helper.GetAppDataPath(), "logs");
    public static readonly string AppDatabasePath = Path.Combine(Helper.GetAppDataPath(), "Schemventory.db");

    public static readonly Color MissingColor = Color.FromArgb(217, 74, 74);
    public static readonly Color CollectedColor = Color.FromArgb(71, 213, 166);
    public static readonly Color IgnoredColor = Color.FromArgb(215, 172, 97);
    public static readonly Color ReplacedColor = Color.FromArgb(43, 133, 187);
    public static readonly Color DefaultColor = Color.FromArgb(58, 55, 55);
    public static readonly Color DefaultBorderColor = Color.FromArgb(103, 99, 99);
    public static readonly Color MaterialDragPlaceholderColor = Color.FromArgb(162, 123, 90);
    public static readonly Color MaterialDragPlaceholderBackgroundColor = Color.FromArgb(25, 194, 165, 142);
}