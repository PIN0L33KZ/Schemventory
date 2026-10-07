namespace Schemventory.Data;

public sealed class BlockStateCount
{
    public required BlockStateEntry BlockState { get; init; }
    public long Count { get; init; }
}