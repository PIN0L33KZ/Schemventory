using fNbt;
using Schemventory.Data;
using Schemventory.Interfaces;

namespace Schemventory.Services;

public sealed class LitematicReader : ISchematicReader
{
    public bool CanRead(string filePath)
    {
        return Path.GetExtension(filePath).Equals(".litematic", StringComparison.OrdinalIgnoreCase);
    }

    public SchematicData Read(string filePath)
    {
        NbtFile nbtFile = new();
        _ = nbtFile.LoadFromFile(filePath);

        NbtCompound regions = nbtFile.RootTag.Get<NbtCompound>("Regions") ??
            throw new InvalidDataException("Regions compound is missing.");

        Dictionary<string, BlockStateCounter> blockStates = new(StringComparer.Ordinal);

        foreach(NbtTag tag in regions)
        {
            if(tag is NbtCompound region)
                ReadRegion(region, blockStates);
        }

        return new SchematicData
        {
            Format = "Litematic",
            BlockStates = blockStates.Values.Select(x => new BlockStateCount
            {
                BlockState = x.BlockState,
                Count = x.Count
            }).ToList()
        };
    }

    private static void ReadRegion(NbtCompound region, Dictionary<string, BlockStateCounter> result)
    {
        NbtList paletteTag = region.Get<NbtList>("BlockStatePalette") ??
            throw new InvalidDataException("BlockStatePalette is missing.");

        NbtLongArray blockStatesTag = region.Get<NbtLongArray>("BlockStates") ??
            throw new InvalidDataException("BlockStates is missing.");

        NbtCompound size = region.Get<NbtCompound>("Size") ??
            throw new InvalidDataException("Size is missing.");

        List<BlockStateEntry> palette = ReadPalette(paletteTag);

        var sizeX = Math.Abs(ReadInt(size, "x"));
        var sizeY = Math.Abs(ReadInt(size, "y"));
        var sizeZ = Math.Abs(ReadInt(size, "z"));

        var blockCount = (long)sizeX * sizeY * sizeZ;
        var bitsPerBlock = CalculateBitsPerBlock(palette.Count);

        for(long blockIndex = 0; blockIndex < blockCount; blockIndex++)
        {
            var paletteIndex = ReadPackedPaletteIndex(blockStatesTag.Value, blockIndex, bitsPerBlock);

            if(paletteIndex < 0 || paletteIndex >= palette.Count)
                throw new InvalidDataException($"Invalid palette index {paletteIndex}.");

            BlockStateEntry blockState = palette[paletteIndex];
            var key = CreateBlockStateKey(blockState);

            if(result.TryGetValue(key, out BlockStateCounter? counter))
                counter.Count++;
            else
                result[key] = new BlockStateCounter(blockState, 1);
        }
    }

    private static List<BlockStateEntry> ReadPalette(NbtList paletteTag)
    {
        List<BlockStateEntry> palette = [];

        foreach(NbtTag tag in paletteTag)
        {
            if(tag is not NbtCompound blockState)
                continue;

            NbtString name = blockState.Get<NbtString>("Name") ??
                throw new InvalidDataException("Palette entry does not contain a Name.");

            Dictionary<string, string> properties = new(StringComparer.Ordinal);
            NbtCompound? propertiesTag = blockState.Get<NbtCompound>("Properties");

            if(propertiesTag is not null)
            {
                foreach(NbtTag propertyTag in propertiesTag)
                {
                    if(propertyTag is NbtString property)
                        properties[property.Name] = property.Value;
                }
            }

            palette.Add(new BlockStateEntry
            {
                Id = name.Value,
                Properties = properties
            });
        }

        return palette;
    }

    private static string CreateBlockStateKey(BlockStateEntry blockState)
    {
        if(blockState.Properties.Count == 0)
            return blockState.Id;

        var properties = string.Join(",", blockState.Properties.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}"));
        return $"{blockState.Id}[{properties}]";
    }

    private static int CalculateBitsPerBlock(int paletteCount)
    {
        if(paletteCount <= 1)
            return 2;

        var bits = 0;
        var value = paletteCount - 1;

        while(value > 0)
        {
            bits++;
            value >>= 1;
        }

        return Math.Max(2, bits);
    }

    private static int ReadPackedPaletteIndex(long[] data, long blockIndex, int bitsPerBlock)
    {
        var bitIndex = blockIndex * bitsPerBlock;
        var startLongIndex = (int)(bitIndex >> 6);
        var startBitOffset = (int)(bitIndex & 63);
        var mask = (1UL << bitsPerBlock) - 1UL;
        var firstLong = unchecked((ulong)data[startLongIndex]);

        if(startBitOffset + bitsPerBlock <= 64)
            return (int)((firstLong >> startBitOffset) & mask);

        var bitsFromFirstLong = 64 - startBitOffset;
        var bitsFromSecondLong = bitsPerBlock - bitsFromFirstLong;
        var firstPart = firstLong >> startBitOffset;
        var secondLong = unchecked((ulong)data[startLongIndex + 1]);
        var secondMask = (1UL << bitsFromSecondLong) - 1UL;
        var secondPart = secondLong & secondMask;

        return (int)((firstPart | (secondPart << bitsFromFirstLong)) & mask);
    }

    private static int ReadInt(NbtCompound compound, string name)
    {
        return compound.Get<NbtInt>(name)?.Value ??
            throw new InvalidDataException($"Missing tag '{name}'.");
    }

    private sealed class BlockStateCounter
    {
        public BlockStateEntry BlockState { get; }
        public long Count { get; set; }

        public BlockStateCounter(BlockStateEntry blockState, long count)
        {
            BlockState = blockState;
            Count = count;
        }
    }
}