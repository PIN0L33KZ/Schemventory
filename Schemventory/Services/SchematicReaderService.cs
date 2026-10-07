using Schemventory.Data;
using Schemventory.Interfaces;

namespace Schemventory.Services;

public sealed class SchematicReaderService
{
    private readonly IReadOnlyCollection<ISchematicReader> _readers;

    public SchematicReaderService(IEnumerable<ISchematicReader> readers)
    {
        _readers = readers.ToList();
    }

    public SchematicData Read(string filePath)
    {
        if(string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));

        if(!File.Exists(filePath))
            throw new FileNotFoundException("Schematic file does not exist.", filePath);

        ISchematicReader? reader = _readers.FirstOrDefault(x => x.CanRead(filePath));

        return reader is null
            ? throw new NotSupportedException($"Schematic format '{Path.GetExtension(filePath)}' is not supported.")
            : reader.Read(filePath);
    }
}