using Serilog;
using Schemventory.Data;
using Schemventory.Interfaces;

namespace Schemventory.Services;

public sealed class MaterialImportService
{
    private const string LogContext = "(MaterialImportService)";

    private readonly SchematicReaderService _schematicReaderService;
    private readonly MaterialListService _materialListService;

    public MaterialImportService()
    {
        BlockStateParser blockStateParser = new();
        MaterialResolver materialResolver = new();

        ISchematicReader[] readers =
        [
            new LitematicReader(),
            new SpongeSchematicReader(blockStateParser)
        ];

        _schematicReaderService = new SchematicReaderService(readers);
        _materialListService = new MaterialListService(materialResolver);
    }

    public IReadOnlyCollection<MaterialEntry> ReadMaterialList(string filePath)
    {
        Log.Debug("{LogContext} Reading schematic material list. Path={Path}", LogContext, filePath);

        try
        {
            SchematicData schematicData = _schematicReaderService.Read(filePath);
            IReadOnlyCollection<MaterialEntry> materials = _materialListService.CreateMaterialList(schematicData);

            Log.Debug("{LogContext} Material list generated. Format={Format}, MaterialCount={MaterialCount}", LogContext, schematicData.Format, materials.Count);

            return materials;
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to generate material list. Path={Path}", LogContext, filePath);
            throw;
        }
    }
}
