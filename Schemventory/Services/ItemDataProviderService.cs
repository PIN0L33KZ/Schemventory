using Serilog;
using Schemventory.App;
using Schemventory.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Schemventory.Services;

public sealed class ItemDataProvider
{
    private const string LogContext = "(ItemDataProvider)";
    private const string DataUrl = @"https://raw.githubusercontent.com/misode/mcmeta/summary/item_components/data.json";

    private static readonly TimeSpan CacheLifetime = TimeSpan.FromDays(7);

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly string _cacheFilePath;
    private readonly string _cacheMetadataFilePath;
    private readonly string _legacyCacheFilePath;
    private readonly SemaphoreSlim _loadSemaphore = new(1, 1);

    private IReadOnlyCollection<ItemData>? _items;
    private Dictionary<string, ItemData>? _itemsById;

    public ItemDataProvider()
    {
        _ = Directory.CreateDirectory(Constants.CacheDirectory);

        _cacheFilePath = Path.Combine(Constants.CacheDirectory, "items.json");
        _cacheMetadataFilePath = Path.Combine(Constants.CacheDirectory, "items.meta.json");
        _legacyCacheFilePath = Path.Combine(Constants.CacheDirectory, "item_components.json");

        Log.Debug("{LogContext} Item data cache initialised. CachePath={CachePath}", LogContext, _cacheFilePath);
    }

    public async Task<int> GetMaxStackSizeAsync(string itemId, CancellationToken cancellationToken = default)
    {
        await EnsureLoadedAsync(cancellationToken);

        var normalizedId = NormalizeItemId(itemId);

        return _itemsById!.TryGetValue(normalizedId, out ItemData? item) ? item.MaxStackSize : 64;
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

        await _loadSemaphore.WaitAsync(cancellationToken);

        try
        {
            if(_items is not null && _itemsById is not null)
                return;

            var cacheLoaded = await TryLoadCompactCacheAsync(cancellationToken);

            if(cacheLoaded && await IsCacheFreshAsync(cancellationToken))
            {
                Log.Debug("{LogContext} Using fresh item data cache. ItemCount={ItemCount}", LogContext, _items?.Count ?? 0);
                return;
            }

            if(!cacheLoaded)
                cacheLoaded = await TryMigrateLegacyCacheAsync(cancellationToken);

            try
            {
                CacheMetadata? metadata = await ReadCacheMetadataAsync(cancellationToken);
                DownloadResult refreshResult = await DownloadDataAsync(metadata, cancellationToken);

                if(refreshResult.NotModified)
                {
                    if(cacheLoaded)
                    {
                        await WriteCacheMetadataAsync(new CacheMetadata
                        {
                            LastCheckedUtc = DateTime.UtcNow,
                            ETag = refreshResult.ETag ?? metadata?.ETag,
                            LastModified = refreshResult.LastModified ?? metadata?.LastModified
                        }, cancellationToken);

                        Log.Debug("{LogContext} Item data cache revalidated without changes.", LogContext);
                        return;
                    }

                    refreshResult = await DownloadDataAsync(null, cancellationToken);
                }

                if(refreshResult.Json is not null)
                {
                    LoadSourceData(refreshResult.Json);
                    await WriteCompactCacheAsync(cancellationToken);

                    await WriteCacheMetadataAsync(new CacheMetadata
                    {
                        LastCheckedUtc = DateTime.UtcNow,
                        ETag = refreshResult.ETag,
                        LastModified = refreshResult.LastModified
                    }, cancellationToken);

                    TryDeleteLegacyCache();

                    Log.Information("{LogContext} Item data cache updated from remote source. ItemCount={ItemCount}", LogContext, _items?.Count ?? 0);
                    return;
                }
            }
            catch(HttpRequestException exception)
            {
                if(cacheLoaded)
                {
                    Log.Warning(exception, "{LogContext} Item data refresh failed. Existing cache will be used.", LogContext);
                    return;
                }

                Log.Warning(exception, "{LogContext} Item data download failed and no cache is available.", LogContext);
            }
            catch(OperationCanceledException exception) when(!cancellationToken.IsCancellationRequested)
            {
                if(cacheLoaded)
                {
                    Log.Warning(exception, "{LogContext} Item data refresh timed out. Existing cache will be used.", LogContext);
                    return;
                }

                Log.Warning(exception, "{LogContext} Item data refresh timed out and no cache is available.", LogContext);
            }

            _items = [];
            _itemsById = new Dictionary<string, ItemData>(StringComparer.Ordinal);

            Log.Warning("{LogContext} Item data provider initialised with an empty data set.", LogContext);
        }
        finally
        {
            _ = _loadSemaphore.Release();
        }
    }

    private async Task<bool> TryLoadCompactCacheAsync(CancellationToken cancellationToken)
    {
        if(!File.Exists(_cacheFilePath))
            return false;

        try
        {
            var json = await File.ReadAllTextAsync(_cacheFilePath, cancellationToken);
            Dictionary<string, int>? stackSizes = JsonSerializer.Deserialize<Dictionary<string, int>>(json);

            if(stackSizes is null)
                return false;

            LoadCompactData(stackSizes);
            Log.Debug("{LogContext} Item data loaded from compact cache. ItemCount={ItemCount}", LogContext, stackSizes.Count);
            return true;
        }
        catch(JsonException exception)
        {
            Log.Warning(exception, "{LogContext} Compact item data cache is invalid and will be removed.", LogContext);
            TryDeleteFile(_cacheFilePath);
            TryDeleteFile(_cacheMetadataFilePath);
            return false;
        }
        catch(IOException exception)
        {
            Log.Warning(exception, "{LogContext} Failed to read compact item data cache.", LogContext);
            return false;
        }
        catch(UnauthorizedAccessException exception)
        {
            Log.Warning(exception, "{LogContext} Access to the compact item data cache was denied.", LogContext);
            return false;
        }
    }

    private async Task<bool> TryMigrateLegacyCacheAsync(CancellationToken cancellationToken)
    {
        if(!File.Exists(_legacyCacheFilePath))
            return false;

        try
        {
            var json = await File.ReadAllTextAsync(_legacyCacheFilePath, cancellationToken);

            LoadSourceData(json);
            await WriteCompactCacheAsync(cancellationToken);

            await WriteCacheMetadataAsync(new CacheMetadata
            {
                LastCheckedUtc = File.GetLastWriteTimeUtc(_legacyCacheFilePath)
            }, cancellationToken);

            TryDeleteLegacyCache();

            Log.Information("{LogContext} Legacy item data cache migrated. ItemCount={ItemCount}", LogContext, _items?.Count ?? 0);
            return true;
        }
        catch(JsonException exception)
        {
            Log.Warning(exception, "{LogContext} Legacy item data cache is invalid and will be removed.", LogContext);
            TryDeleteLegacyCache();
            return false;
        }
        catch(IOException exception)
        {
            Log.Warning(exception, "{LogContext} Failed to migrate legacy item data cache.", LogContext);
            return false;
        }
        catch(UnauthorizedAccessException exception)
        {
            Log.Warning(exception, "{LogContext} Access to the legacy item data cache was denied.", LogContext);
            return false;
        }
    }

    private async Task<bool> IsCacheFreshAsync(CancellationToken cancellationToken)
    {
        CacheMetadata? metadata = await ReadCacheMetadataAsync(cancellationToken);

        return metadata is not null
            ? DateTime.UtcNow - metadata.LastCheckedUtc < CacheLifetime
            : File.Exists(_cacheFilePath) && DateTime.UtcNow - File.GetLastWriteTimeUtc(_cacheFilePath) < CacheLifetime;
    }

    private void LoadSourceData(string json)
    {
        Dictionary<string, int> stackSizes = new(StringComparer.Ordinal);

        using JsonDocument document = JsonDocument.Parse(json);

        foreach(JsonProperty item in document.RootElement.EnumerateObject())
        {
            var maxStackSize = 64;

            if(item.Value.TryGetProperty("minecraft:max_stack_size", out JsonElement stackSizeElement) && stackSizeElement.TryGetInt32(out var parsedMaxStackSize))
                maxStackSize = parsedMaxStackSize;

            stackSizes[NormalizeItemId(item.Name)] = maxStackSize;
        }

        LoadCompactData(stackSizes);
    }

    private void LoadCompactData(IReadOnlyDictionary<string, int> stackSizes)
    {
        List<ItemData> items = new(stackSizes.Count);
        Dictionary<string, ItemData> itemsById = new(stackSizes.Count, StringComparer.Ordinal);

        foreach(KeyValuePair<string, int> entry in stackSizes)
        {
            ItemData itemData = new()
            {
                Id = NormalizeItemId(entry.Key),
                MaxStackSize = entry.Value
            };

            items.Add(itemData);
            itemsById[itemData.Id] = itemData;
        }

        _items = items.OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
        _itemsById = itemsById;
    }

    private async Task WriteCompactCacheAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, int> stackSizes = _items!.ToDictionary(x => x.Id, x => x.MaxStackSize, StringComparer.Ordinal);
        var json = JsonSerializer.Serialize(stackSizes);

        await File.WriteAllTextAsync(_cacheFilePath, json, cancellationToken);
    }

    private async Task<CacheMetadata?> ReadCacheMetadataAsync(CancellationToken cancellationToken)
    {
        if(!File.Exists(_cacheMetadataFilePath))
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(_cacheMetadataFilePath, cancellationToken);

            return JsonSerializer.Deserialize<CacheMetadata>(json);
        }
        catch(JsonException exception)
        {
            Log.Warning(exception, "{LogContext} Item data cache metadata is invalid and will be removed.", LogContext);
            TryDeleteFile(_cacheMetadataFilePath);
            return null;
        }
        catch(IOException exception)
        {
            Log.Debug(exception, "{LogContext} Failed to read item data cache metadata.", LogContext);
            return null;
        }
        catch(UnauthorizedAccessException exception)
        {
            Log.Debug(exception, "{LogContext} Access to item data cache metadata was denied.", LogContext);
            return null;
        }
    }

    private async Task WriteCacheMetadataAsync(CacheMetadata metadata, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(metadata);

        await File.WriteAllTextAsync(_cacheMetadataFilePath, json, cancellationToken);
    }

    private static string NormalizeItemId(string itemId)
    {
        const string prefix = "minecraft:";

        return itemId.StartsWith(prefix, StringComparison.Ordinal) ? itemId[prefix.Length..] : itemId;
    }

    private static async Task<DownloadResult> DownloadDataAsync(CacheMetadata? metadata, CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;

        for(var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                Log.Debug("{LogContext} Requesting item data. Attempt={Attempt}", LogContext, attempt);

                using HttpRequestMessage request = new(HttpMethod.Get, DataUrl);

                request.Headers.UserAgent.ParseAdd("Schemventory/1.0");

                if(!string.IsNullOrWhiteSpace(metadata?.ETag))
                    request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(metadata.ETag));

                if(metadata?.LastModified is not null)
                    request.Headers.IfModifiedSince = metadata.LastModified;

                using HttpResponseMessage response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if(response.StatusCode == HttpStatusCode.NotModified)
                    return new DownloadResult(null, true, response.Headers.ETag?.Tag, response.Content.Headers.LastModified);

                if(response.StatusCode == HttpStatusCode.TooManyRequests)
                    throw new HttpRequestException("Item data request was rate limited.", null, response.StatusCode);

                if((int)response.StatusCode >= 500 && attempt < maxAttempts)
                {
                    Log.Warning("{LogContext} Item data request returned a server error. StatusCode={StatusCode}, Attempt={Attempt}", LogContext, (int)response.StatusCode, attempt);
                    await Task.Delay(TimeSpan.FromMilliseconds(500 * attempt), cancellationToken);
                    continue;
                }

                _ = response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                return new DownloadResult(json, false, response.Headers.ETag?.Tag, response.Content.Headers.LastModified);
            }
            catch(HttpRequestException exception) when(attempt < maxAttempts && exception.StatusCode != HttpStatusCode.TooManyRequests)
            {
                Log.Warning(exception, "{LogContext} Item data request failed. Retrying. Attempt={Attempt}", LogContext, attempt);
                await Task.Delay(TimeSpan.FromMilliseconds(500 * attempt), cancellationToken);
            }
        }

        throw new HttpRequestException("Item data could not be downloaded.");
    }

    private void TryDeleteLegacyCache()
    {
        TryDeleteFile(_legacyCacheFilePath);
    }

    private static void TryDeleteFile(string filePath)
    {
        try
        {
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
        catch(Exception exception)
        {
            Log.Debug(exception, "{LogContext} Cache cleanup failed. Path={Path}", LogContext, filePath);
        }
    }

    private sealed class CacheMetadata
    {
        public DateTime LastCheckedUtc { get; set; }
        public string? ETag { get; set; }
        public DateTimeOffset? LastModified { get; set; }
    }

    private sealed record DownloadResult(string? Json, bool NotModified, string? ETag, DateTimeOffset? LastModified);
}
