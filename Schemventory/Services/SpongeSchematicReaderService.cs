using fNbt;
using Serilog;
using Schemventory.Data;
using Schemventory.Interfaces;

namespace Schemventory.Services;

public sealed class SpongeSchematicReader : ISchematicReader
{
    private const string LogContext = "(SpongeSchematicReader)";

    private readonly BlockStateParser _blockStateParser;

    public SpongeSchematicReader(BlockStateParser blockStateParser)
    {
        _blockStateParser = blockStateParser;
    }

    public bool CanRead(string filePath)
    {
        return Path.GetExtension(filePath).Equals(".schem", StringComparison.OrdinalIgnoreCase);
    }

    public SchematicData Read(string filePath)
    {
        Log.Debug("{LogContext} Reading Sponge schematic file. Path={Path}", LogContext, filePath);

        try
        {
            NbtFile nbtFile = new();
            _ = nbtFile.LoadFromFile(filePath);

            NbtCompound schematic = GetSchematicCompound(nbtFile.RootTag);
            var version = ReadInt(schematic, "Version");

            SchematicData result = version switch
            {
                2 => ReadVersion2(schematic),
                3 => ReadVersion3(schematic),
                _ => throw new NotSupportedException($"Sponge Schematic version {version} is not supported.")
            };

            Log.Debug("{LogContext} Sponge schematic read successfully. Path={Path}, Version={Version}, BlockStateCount={BlockStateCount}", LogContext, filePath, version, result.BlockStates.Count);

            return result;
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to read Sponge schematic file. Path={Path}", LogContext, filePath);
            throw;
        }
    }

    private SchematicData ReadVersion2(NbtCompound schematic)
    {
        NbtCompound palette = schematic.Get<NbtCompound>("Palette") ?? throw new InvalidDataException("Palette is missing.");
        NbtByteArray blockData = schematic.Get<NbtByteArray>("BlockData") ?? throw new InvalidDataException("BlockData is missing.");

        return CreateResult("Sponge Schematic v2", palette, blockData.Value, schematic);
    }

    private SchematicData ReadVersion3(NbtCompound schematic)
    {
        NbtCompound blocks = schematic.Get<NbtCompound>("Blocks") ?? throw new InvalidDataException("Blocks compound is missing.");
        NbtCompound palette = blocks.Get<NbtCompound>("Palette") ?? throw new InvalidDataException("Palette is missing.");
        NbtByteArray blockData = blocks.Get<NbtByteArray>("Data") ?? throw new InvalidDataException("Block data is missing.");

        return CreateResult("Sponge Schematic v3", palette, blockData.Value, schematic);
    }

    private SchematicData CreateResult(string format, NbtCompound paletteTag, byte[] blockData, NbtCompound schematic)
    {
        Dictionary<int, BlockStateEntry> palette = ReadPalette(paletteTag);
        Dictionary<int, long> counts = [];

        var offset = 0;
        long blocksRead = 0;

        while(offset < blockData.Length)
        {
            var paletteIndex = ReadVarInt(blockData, ref offset);
            blocksRead++;

            if(!counts.TryAdd(paletteIndex, 1))
                counts[paletteIndex]++;
        }

        var expectedBlocks = GetExpectedBlockCount(schematic);

        if(blocksRead != expectedBlocks)
            throw new InvalidDataException($"Expected {expectedBlocks} blocks but decoded {blocksRead}.");

        List<BlockStateCount> blockStates = [];

        foreach(KeyValuePair<int, long> entry in counts)
        {
            if(!palette.TryGetValue(entry.Key, out BlockStateEntry? blockState))
                throw new InvalidDataException($"Palette index {entry.Key} does not exist.");

            blockStates.Add(new BlockStateCount
            {
                BlockState = blockState,
                Count = entry.Value
            });
        }

        return new SchematicData
        {
            Format = format,
            BlockStates = blockStates
        };
    }

    private Dictionary<int, BlockStateEntry> ReadPalette(NbtCompound paletteTag)
    {
        Dictionary<int, BlockStateEntry> palette = [];

        foreach(NbtTag tag in paletteTag)
        {
            if(tag is NbtInt paletteIndex)
                palette[paletteIndex.Value] = _blockStateParser.Parse(paletteIndex.Name);
        }

        return palette;
    }

    private static int ReadVarInt(byte[] data, ref int offset)
    {
        var value = 0;
        var position = 0;

        while(true)
        {
            if(offset >= data.Length)
                throw new InvalidDataException("Unexpected end of VarInt data.");

            var currentByte = data[offset++];
            value |= (currentByte & 0x7F) << position;

            if((currentByte & 0x80) == 0)
                return value;

            position += 7;

            if(position >= 35)
                throw new InvalidDataException("VarInt is too large.");
        }
    }

    private static long GetExpectedBlockCount(NbtCompound schematic)
    {
        var width = ReadUnsignedShort(schematic, "Width");
        var height = ReadUnsignedShort(schematic, "Height");
        var length = ReadUnsignedShort(schematic, "Length");

        return (long)width * height * length;
    }

    private static int ReadUnsignedShort(NbtCompound compound, string name)
    {
        NbtShort tag = compound.Get<NbtShort>(name) ?? throw new InvalidDataException($"Missing tag '{name}'.");

        return unchecked((ushort)tag.Value);
    }

    private static int ReadInt(NbtCompound compound, string name)
    {
        return compound.Get<NbtInt>(name)?.Value ?? throw new InvalidDataException($"Missing tag '{name}'.");
    }

    private static NbtCompound GetSchematicCompound(NbtCompound root)
    {
        NbtCompound? schematic = root.Get<NbtCompound>("Schematic");

        return schematic is not null
            ? schematic
            : root.Get<NbtInt>("Version") is not null
            ? root
            : throw new InvalidDataException("The file does not contain a valid Sponge schematic.");
    }
}
