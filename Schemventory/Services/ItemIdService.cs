namespace Schemventory.Services;

public static class ItemIdService
{
    private const string MinecraftNamespace = "minecraft";

    public static string Normalize(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);

        const string prefix = MinecraftNamespace + ":";

        return itemId.StartsWith(prefix, StringComparison.Ordinal)
            ? itemId[prefix.Length..]
            : itemId;
    }

    public static string GetFullId(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);

        return itemId.Contains(':', StringComparison.Ordinal)
            ? itemId
            : $"{MinecraftNamespace}:{itemId}";
    }

    public static string GetDisplayName(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);

        var separatorIndex = itemId.IndexOf(':');

        var name = separatorIndex >= 0
            ? itemId[(separatorIndex + 1)..]
            : itemId;

        name = name.Replace('_', ' ');

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
    }
}