namespace Schemventory.Data;

public sealed class MaterialEntry
{
    public required string Id { get; init; }

    public long BlocksTotal { get; set; }

    public int MaxStackSize { get; set; } = 64;

    public long Stacks => BlocksTotal / MaxStackSize;

    public long Blocks => BlocksTotal % MaxStackSize;
}