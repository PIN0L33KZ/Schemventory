using Schemventory.Data;

namespace Schemventory.Interfaces;

public interface ISchematicReader
{
    bool CanRead(string filePath);
    SchematicData Read(string filePath);
}