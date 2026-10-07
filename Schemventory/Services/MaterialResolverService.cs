using Schemventory.Data;

namespace Schemventory.Services;

public sealed class MaterialResolver
{
    private static readonly IReadOnlyList<ResolvedMaterial> NoMaterials =
        Array.Empty<ResolvedMaterial>();

    private static readonly HashSet<string> IgnoredBlocks = new(StringComparer.Ordinal)
    {
        "minecraft:air",
        "minecraft:cave_air",
        "minecraft:void_air",
        "minecraft:water",
        "minecraft:lava",
        "minecraft:fire",
        "minecraft:soul_fire",
        "minecraft:bubble_column",
        "minecraft:moving_piston",
        "minecraft:piston_head",
        "minecraft:nether_portal",
        "minecraft:end_portal",
        "minecraft:end_gateway",
        "minecraft:light",
        "minecraft:powder_snow"
    };

    private static readonly HashSet<string> DoubleHeightPlants = new(StringComparer.Ordinal)
    {
        "minecraft:sunflower",
        "minecraft:lilac",
        "minecraft:rose_bush",
        "minecraft:peony",
        "minecraft:tall_grass",
        "minecraft:large_fern",
        "minecraft:pitcher_plant"
    };

    private static readonly Dictionary<string, string> RenamedBlockIds =
        new(StringComparer.Ordinal)
        {
            ["minecraft:chain"] = "minecraft:iron_chain"
        };

    private static readonly Dictionary<string, string> BlockToItemMappings =
        new(StringComparer.Ordinal)
        {
            ["minecraft:redstone_wire"] = "minecraft:redstone",
            ["minecraft:tripwire"] = "minecraft:string",

            ["minecraft:wall_torch"] = "minecraft:torch",
            ["minecraft:redstone_wall_torch"] = "minecraft:redstone_torch",
            ["minecraft:soul_wall_torch"] = "minecraft:soul_torch",

            ["minecraft:wheat"] = "minecraft:wheat_seeds",
            ["minecraft:beetroots"] = "minecraft:beetroot_seeds",
            ["minecraft:carrots"] = "minecraft:carrot",
            ["minecraft:potatoes"] = "minecraft:potato",
            ["minecraft:melon_stem"] = "minecraft:melon_seeds",
            ["minecraft:attached_melon_stem"] = "minecraft:melon_seeds",
            ["minecraft:pumpkin_stem"] = "minecraft:pumpkin_seeds",
            ["minecraft:attached_pumpkin_stem"] = "minecraft:pumpkin_seeds",
            ["minecraft:cocoa"] = "minecraft:cocoa_beans",
            ["minecraft:sweet_berry_bush"] = "minecraft:sweet_berries",
            ["minecraft:torchflower_crop"] = "minecraft:torchflower_seeds",
            ["minecraft:pitcher_crop"] = "minecraft:pitcher_pod",

            ["minecraft:water_cauldron"] = "minecraft:cauldron",
            ["minecraft:lava_cauldron"] = "minecraft:cauldron",
            ["minecraft:powder_snow_cauldron"] = "minecraft:cauldron",

            ["minecraft:dirt_path"] = "minecraft:dirt",
            ["minecraft:farmland"] = "minecraft:dirt"
        };

    public IReadOnlyList<ResolvedMaterial> Resolve(BlockStateEntry blockState)
    {
        ArgumentNullException.ThrowIfNull(blockState);

        var blockId = NormalizeBlockId(blockState.Id);

        if(IgnoredBlocks.Contains(blockId))
            return NoMaterials;

        if(blockId.EndsWith("_door", StringComparison.Ordinal))
        {
            var half = blockState.GetProperty("half");

            return half == "upper"
                ? NoMaterials
                : Single(blockId);
        }

        if(blockId.EndsWith("_bed", StringComparison.Ordinal))
        {
            var part = blockState.GetProperty("part");

            return part == "head"
                ? NoMaterials
                : Single(blockId);
        }

        if(DoubleHeightPlants.Contains(blockId))
        {
            var half = blockState.GetProperty("half");

            return half == "upper"
                ? NoMaterials
                : Single(blockId);
        }

        if(blockId.StartsWith("minecraft:potted_", StringComparison.Ordinal))
            return ResolvePottedPlant(blockId);

        if(blockId == "minecraft:candle_cake")
        {
            return
            [
                Create("minecraft:cake"),
                Create("minecraft:candle")
            ];
        }

        if(blockId.EndsWith("_candle_cake", StringComparison.Ordinal))
            return ResolveCandleCake(blockId);

        var wallVariant = ResolveWallVariant(blockId);

        return wallVariant is not null
            ? Single(wallVariant)
            : BlockToItemMappings.TryGetValue(blockId, out var itemId) ? Single(itemId) : Single(blockId);
    }

    private static string NormalizeBlockId(string blockId)
    {
        return RenamedBlockIds.TryGetValue(blockId, out var currentId)
            ? currentId
            : blockId;
    }

    private static IReadOnlyList<ResolvedMaterial> ResolvePottedPlant(string blockId)
    {
        const string prefix = "minecraft:potted_";

        var plantName = blockId[prefix.Length..];

        var itemId = plantName switch
        {
            "azalea_bush" => "minecraft:azalea",
            "flowering_azalea_bush" => "minecraft:flowering_azalea",
            _ => $"minecraft:{plantName}"
        };

        return
        [
            Create("minecraft:flower_pot"),
            Create(itemId)
        ];
    }

    private static IReadOnlyList<ResolvedMaterial> ResolveCandleCake(string blockId)
    {
        const string minecraftPrefix = "minecraft:";
        const string cakeSuffix = "_cake";

        var candleName = blockId[minecraftPrefix.Length..^cakeSuffix.Length];

        return
        [
            Create("minecraft:cake"),
            Create($"minecraft:{candleName}")
        ];
    }

    private static string? ResolveWallVariant(string blockId)
    {
        if(blockId.EndsWith("_wall_sign", StringComparison.Ordinal))
        {
            return blockId.Replace(
                "_wall_sign",
                "_sign",
                StringComparison.Ordinal);
        }

        return blockId.EndsWith("_wall_hanging_sign", StringComparison.Ordinal)
            ? blockId.Replace(
                "_wall_hanging_sign",
                "_hanging_sign",
                StringComparison.Ordinal)
            : blockId.EndsWith("_wall_banner", StringComparison.Ordinal)
            ? blockId.Replace(
                "_wall_banner",
                "_banner",
                StringComparison.Ordinal)
            : blockId switch
            {
                "minecraft:skeleton_wall_skull" => "minecraft:skeleton_skull",
                "minecraft:wither_skeleton_wall_skull" => "minecraft:wither_skeleton_skull",
                "minecraft:zombie_wall_head" => "minecraft:zombie_head",
                "minecraft:player_wall_head" => "minecraft:player_head",
                "minecraft:creeper_wall_head" => "minecraft:creeper_head",
                "minecraft:dragon_wall_head" => "minecraft:dragon_head",
                "minecraft:piglin_wall_head" => "minecraft:piglin_head",

                _ when blockId.Contains("_wall_coral_fan", StringComparison.Ordinal) =>
                    blockId.Replace(
                        "_wall_coral_fan",
                        "_coral_fan",
                        StringComparison.Ordinal),

                _ => null
            };
    }

    private static IReadOnlyList<ResolvedMaterial> Single(string id)
    {
        return [Create(id)];
    }

    private static ResolvedMaterial Create(string id, long count = 1)
    {
        return new ResolvedMaterial
        {
            Id = id,
            Count = count
        };
    }
}