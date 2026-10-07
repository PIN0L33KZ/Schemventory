namespace Schemventory.App;

internal class Constants
{
    public const string AppName = "Schemventory";
    public static readonly string CacheDirectory = Path.Combine(Helper.GetAppDataPath(), "Cache");
    public static readonly string AppDatabasePath = Path.Combine(Helper.GetAppDataPath(), "Schemventory.db");

    public static readonly Color MissingColor = Color.FromArgb(217, 74, 74);
    public static readonly Color CollectedColor = Color.FromArgb(71, 213, 166);
    public static readonly Color IgnoredColor = Color.FromArgb(215, 172, 97);
    public static readonly Color ReplacedColor = Color.FromArgb(43, 133, 187);
    public static readonly Color DefaultColor = Color.FromArgb(58, 55, 55);
    public static readonly Color DefaultBorderColor = Color.FromArgb(103, 99, 99);
}