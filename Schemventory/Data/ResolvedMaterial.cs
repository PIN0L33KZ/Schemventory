namespace Schemventory.Data;

public sealed class ResolvedMaterial
{
    public required string Id { get; init; }

    public long Count { get; init; } = 1;
}