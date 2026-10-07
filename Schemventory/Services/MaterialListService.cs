using Schemventory.Data;

namespace Schemventory.Services;

public sealed class MaterialListService
{
    private readonly MaterialResolver _materialResolver;

    public MaterialListService(MaterialResolver materialResolver)
    {
        _materialResolver = materialResolver;
    }

    public IReadOnlyCollection<MaterialEntry> CreateMaterialList(SchematicData schematic)
    {
        Dictionary<string, long> materialCounts = new(StringComparer.Ordinal);

        foreach(BlockStateCount blockStateCount in schematic.BlockStates)
        {
            IReadOnlyList<ResolvedMaterial> resolvedMaterials =
                _materialResolver.Resolve(blockStateCount.BlockState);

            foreach(ResolvedMaterial material in resolvedMaterials)
            {
                var amount = material.Count * blockStateCount.Count;

                if(!materialCounts.TryAdd(material.Id, amount))
                    materialCounts[material.Id] += amount;
            }
        }

        return materialCounts
            .Select(x => new MaterialEntry
            {
                Id = x.Key,
                BlocksTotal = x.Value
            })
            .OrderByDescending(x => x.BlocksTotal)
            .ThenBy(x => x.Id)
            .ToList();
    }
}