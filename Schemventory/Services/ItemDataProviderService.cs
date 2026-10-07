using Schemventory.App;
using Schemventory.Data;
using System.Text.Json;

namespace Schemventory.Services;

public sealed class ItemDataProvider
{
    private const string DataUrl = @"https://raw.githubusercontent.com/misode/mcmeta/summary/item_components/data.json";

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly string _cacheFilePath;

    private IReadOnlyCollection<ItemData>? _items;
    private Dictionary<string, ItemData>? _itemsById;

    public ItemDataProvider()
    {
        var cacheDirectory = Path.Combine(Helper.GetAppDataPath(), "Cache");

        _ = Directory.CreateDirectory(cacheDirectory);

        _cacheFilePath = Path.Combine(cacheDirectory, "item_components.json");
    }

    public async Task<int> GetMaxStackSizeAsync(string itemId, CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);

        var normalizedId = NormalizeItemId(itemId);

        return _itemsById!.TryGetValue(normalizedId, out ItemData? item)
            ? item.MaxStackSize
            : 64;
    }

    public async Task<IReadOnlyCollection<ItemData>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);

        return _items!;
    }

    private async Task EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        if(_items is not null && _itemsById is not null)
            return;

        if(File.Exists(_cacheFilePath))
        {
            try
            {
                var cachedJson = await File.ReadAllTextAsync(_cacheFilePath, cancellationToken);

                LoadItemData(cachedJson);

                return;
            }
            catch
            {
                // Cache beschädigt oder nicht lesbar.
            }
        }

        try
        {
            var json = await DownloadDataAsync(cancellationToken);

            LoadItemData(json);

            await File.WriteAllTextAsync(_cacheFilePath, json, cancellationToken);
        }
        catch(HttpRequestException)
        {
            _items = [];
            _itemsById = new Dictionary<string, ItemData>(StringComparer.Ordinal);
        }
    }

    private void LoadItemData(string json)
    {
        List<ItemData> items = [];
        Dictionary<string, ItemData> itemsById = new(StringComparer.Ordinal);

        using JsonDocument document = JsonDocument.Parse(json);

        foreach(JsonProperty item in document.RootElement.EnumerateObject())
        {
            var maxStackSize = 64;

            if(item.Value.TryGetProperty("minecraft:max_stack_size", out JsonElement stackSizeElement) &&
               stackSizeElement.TryGetInt32(out var parsedMaxStackSize))
            {
                maxStackSize = parsedMaxStackSize;
            }

            ItemData itemData = new()
            {
                Id = NormalizeItemId(item.Name),
                MaxStackSize = maxStackSize
            };

            items.Add(itemData);
            itemsById[itemData.Id] = itemData;
        }

        _items = items
            .OrderBy(x => x.Id, StringComparer.Ordinal)
            .ToArray();

        _itemsById = itemsById;
    }

    private static string NormalizeItemId(string itemId)
    {
        const string prefix = "minecraft:";

        return itemId.StartsWith(prefix, StringComparison.Ordinal)
            ? itemId[prefix.Length..]
            : itemId;
    }

    private static async Task<string> DownloadDataAsync(CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;

        for(var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, DataUrl);

                request.Headers.UserAgent.ParseAdd("Schemventory/1.0");

                using HttpResponseMessage response = await HttpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                _ = response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch(HttpRequestException) when(attempt < maxAttempts)
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(500 * attempt),
                    cancellationToken);
            }
        }

        throw new HttpRequestException("Item data could not be downloaded.");
    }
}