using Schemventory.Data;
using Schemventory.Interfaces;

namespace Schemventory.Services;

public sealed class MaterialImportService
{
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
        SchematicData schematicData = _schematicReaderService.Read(filePath);

        return _materialListService.CreateMaterialList(schematicData);
    }
}