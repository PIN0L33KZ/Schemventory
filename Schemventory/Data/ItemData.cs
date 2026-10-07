namespace Schemventory.Data;

public sealed class ItemData
{
    public required string Id { get; init; }

    public int MaxStackSize { get; init; } = 64;
}