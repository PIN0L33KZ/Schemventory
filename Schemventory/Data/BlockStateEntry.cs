namespace Schemventory.Data;

public sealed class BlockStateEntry
{
    public required string Id { get; init; }

    public IReadOnlyDictionary<string, string> Properties { get; init; } =
        new Dictionary<string, string>();

    public string? GetProperty(string name)
    {
        return Properties.TryGetValue(name, out var value) ? value : null;
    }
}