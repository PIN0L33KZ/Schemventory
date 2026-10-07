using Schemventory.Data;

namespace Schemventory.Services;

public sealed class BlockStateParser
{
    public BlockStateEntry Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var propertyStart = value.IndexOf('[');

        if(propertyStart < 0)
        {
            return new BlockStateEntry
            {
                Id = NormalizeBlockId(value)
            };
        }

        var propertyEnd = value.LastIndexOf(']');

        if(propertyEnd <= propertyStart)
            throw new InvalidDataException($"Invalid block state: {value}");

        var blockId = value[..propertyStart];
        var propertyText = value[(propertyStart + 1)..propertyEnd];

        Dictionary<string, string> properties = new(StringComparer.Ordinal);

        if(!string.IsNullOrWhiteSpace(propertyText))
        {
            foreach(var entry in propertyText.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var separator = entry.IndexOf('=');

                if(separator <= 0)
                    throw new InvalidDataException($"Invalid block state property: {entry}");

                properties[entry[..separator]] = entry[(separator + 1)..];
            }
        }

        return new BlockStateEntry
        {
            Id = NormalizeBlockId(blockId),
            Properties = properties
        };
    }

    private static string NormalizeBlockId(string blockId)
    {
        return blockId.Contains(':') ? blockId : $"minecraft:{blockId}";
    }
}