using Serilog;
using Schemventory.Data;
using Schemventory.Interfaces;

namespace Schemventory.Services;

public sealed class SchematicReaderService
{
    private const string LogContext = "(SchematicReaderService)";

    private readonly IReadOnlyCollection<ISchematicReader> _readers;

    public SchematicReaderService(IEnumerable<ISchematicReader> readers)
    {
        _readers = readers.ToList();
    }

    public SchematicData Read(string filePath)
    {
        if(string.IsNullOrWhiteSpace(filePath))
        {
            Log.Warning("{LogContext} Read failed because the file path is empty.", LogContext);
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        if(!File.Exists(filePath))
        {
            Log.Warning("{LogContext} Schematic file does not exist. Path={Path}", LogContext, filePath);
            throw new FileNotFoundException("Schematic file does not exist.", filePath);
        }

        ISchematicReader? reader = _readers.FirstOrDefault(x => x.CanRead(filePath));

        if(reader is null)
        {
            Log.Warning("{LogContext} Unsupported schematic format. Path={Path}, Extension={Extension}", LogContext, filePath, Path.GetExtension(filePath));
            throw new NotSupportedException($"Schematic format '{Path.GetExtension(filePath)}' is not supported.");
        }

        Log.Debug("{LogContext} Using reader {ReaderType}. Path={Path}", LogContext, reader.GetType().Name, filePath);

        return reader.Read(filePath);
    }
}
