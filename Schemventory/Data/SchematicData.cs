namespace Schemventory.Data;

public sealed class SchematicData
{
    public required string Format { get; init; }
    public required IReadOnlyCollection<BlockStateCount> BlockStates { get; init; }
}