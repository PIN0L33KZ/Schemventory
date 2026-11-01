using Schemventory.App;
using Serilog;
using System.Collections.Concurrent;
using System.Net;

namespace Schemventory.Services;

public sealed class ItemIconService
{
    private const string LogContext = "(ItemIconService)";
    private const int MaxMemoryCacheEntries = 256;

    private static readonly TimeSpan MissingCacheLifetime = TimeSpan.FromDays(7);

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly string _cacheDirectory;
    private readonly SemaphoreSlim _downloadSemaphore = new(6);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _itemLocks = new(StringComparer.Ordinal);
    private readonly object _memoryCacheLock = new();
    private readonly Dictionary<string, MemoryCacheEntry> _memoryCache = new(StringComparer.Ordinal);
    private readonly LinkedList<string> _memoryCacheUsage = [];

    public ItemIconService()
    {
        _cacheDirectory = Path.Combine(Constants.CacheDirectory, "ItemIcons");

        _ = Directory.CreateDirectory(_cacheDirectory);

        Log.Debug("{LogContext} Item icon cache initialised. CacheDirectory={CacheDirectory}", LogContext, _cacheDirectory);
    }

    public async Task<Image> GetItemIconAsync(string itemId, int size = 64, CancellationToken cancellationToken = default)
    {
        var normalizedItemId = NormalizeItemId(itemId);
        var cacheKey = $"{normalizedItemId}:{size}";
        var cacheFilePath = GetCacheFilePath(normalizedItemId, size);
        var missingCacheFilePath = GetMissingCacheFilePath(normalizedItemId, size);

        cancellationToken.ThrowIfCancellationRequested();

        Image? memoryCachedImage = TryGetMemoryCachedImage(cacheKey);

        if(memoryCachedImage is not null)
            return memoryCachedImage;

        Image? cachedImage = await TryLoadCachedImageAsync(cacheKey, cacheFilePath, cancellationToken);

        if(cachedImage is not null)
            return cachedImage;

        if(IsMissingCacheValid(missingCacheFilePath))
            return new Bitmap(Properties.Resources.MissingIcon);

        SemaphoreSlim itemLock = _itemLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

        await itemLock.WaitAsync(cancellationToken);

        try
        {
            memoryCachedImage = TryGetMemoryCachedImage(cacheKey);

            if(memoryCachedImage is not null)
                return memoryCachedImage;

            cachedImage = await TryLoadCachedImageAsync(cacheKey, cacheFilePath, cancellationToken);

            if(cachedImage is not null)
                return cachedImage;

            if(IsMissingCacheValid(missingCacheFilePath))
                return new Bitmap(Properties.Resources.MissingIcon);

            Log.Debug("{LogContext} Downloading item icon. ItemId={ItemId}, Size={Size}", LogContext, normalizedItemId, size);

            var imageData = await DownloadIconAsync(normalizedItemId, size, cancellationToken);

            if(imageData is null)
            {
                await File.WriteAllTextAsync(missingCacheFilePath, string.Empty, cancellationToken);

                Log.Debug("{LogContext} No remote icon available. ItemId={ItemId}, Size={Size}", LogContext, normalizedItemId, size);
                return new Bitmap(Properties.Resources.MissingIcon);
            }

            TryDeleteFile(missingCacheFilePath);

            await File.WriteAllBytesAsync(cacheFilePath, imageData, cancellationToken);

            return CacheAndCloneImage(cacheKey, imageData);
        }
        catch(OperationCanceledException)
        {
            throw;
        }
        catch(Exception exception)
        {
            Log.Warning(exception, "{LogContext} Failed to obtain item icon. ItemId={ItemId}, Size={Size}", LogContext, normalizedItemId, size);
            throw;
        }
        finally
        {
            _ = itemLock.Release();
        }
    }

    private async Task<byte[]?> DownloadIconAsync(string itemId, int size, CancellationToken cancellationToken)
    {
        await _downloadSemaphore.WaitAsync(cancellationToken);

        try
        {
            var imageData = await TryDownloadIconAsync("item", itemId, size, cancellationToken);

            return imageData is not null ? imageData : await TryDownloadIconAsync("block", itemId, size, cancellationToken);
        }
        finally
        {
            _ = _downloadSemaphore.Release();
        }
    }

    private static async Task<byte[]?> TryDownloadIconAsync(string type, string itemId, int size, CancellationToken cancellationToken)
    {
        var url = $"https://blockrender.dev/render/{type}/{Uri.EscapeDataString(itemId)}.png?size={size}";

        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd("Schemventory/1.0");

        using HttpResponseMessage response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if(response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if(response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new HttpRequestException("Item icon request was rate limited.", null, response.StatusCode);

        _ = response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    private Image? TryGetMemoryCachedImage(string cacheKey)
    {
        lock(_memoryCacheLock)
        {
            if(!_memoryCache.TryGetValue(cacheKey, out MemoryCacheEntry? entry))
                return null;

            _memoryCacheUsage.Remove(entry.UsageNode);
            _memoryCacheUsage.AddLast(entry.UsageNode);

            return new Bitmap(entry.Bitmap);
        }
    }

    private async Task<Image?> TryLoadCachedImageAsync(string cacheKey, string filePath, CancellationToken cancellationToken)
    {
        if(!File.Exists(filePath))
            return null;

        try
        {
            var data = await File.ReadAllBytesAsync(filePath, cancellationToken);

            return CacheAndCloneImage(cacheKey, data);
        }
        catch(OperationCanceledException)
        {
            throw;
        }
        catch(Exception exception)
        {
            Log.Debug(exception, "{LogContext} Cached icon could not be loaded and will be removed. Path={Path}", LogContext, filePath);
            TryDeleteFile(filePath);
            return null;
        }
    }

    private Image CacheAndCloneImage(string cacheKey, byte[] data)
    {
        Bitmap createdBitmap = LoadImage(data);

        lock(_memoryCacheLock)
        {
            if(_memoryCache.TryGetValue(cacheKey, out MemoryCacheEntry? existingEntry))
            {
                createdBitmap.Dispose();

                _memoryCacheUsage.Remove(existingEntry.UsageNode);
                _memoryCacheUsage.AddLast(existingEntry.UsageNode);

                return new Bitmap(existingEntry.Bitmap);
            }

            LinkedListNode<string> usageNode = _memoryCacheUsage.AddLast(cacheKey);

            _memoryCache[cacheKey] = new MemoryCacheEntry
            {
                Bitmap = createdBitmap,
                UsageNode = usageNode
            };

            TrimMemoryCache();

            return new Bitmap(createdBitmap);
        }
    }

    private void TrimMemoryCache()
    {
        while(_memoryCache.Count > MaxMemoryCacheEntries)
        {
            LinkedListNode<string>? oldestNode = _memoryCacheUsage.First;

            if(oldestNode is null)
                return;

            _memoryCacheUsage.RemoveFirst();

            if(!_memoryCache.Remove(oldestNode.Value, out MemoryCacheEntry? entry))
                continue;

            entry.Bitmap.Dispose();
        }
    }

    private static bool IsMissingCacheValid(string filePath)
    {
        if(!File.Exists(filePath))
            return false;

        try
        {
            if(DateTime.UtcNow - File.GetLastWriteTimeUtc(filePath) < MissingCacheLifetime)
                return true;

            File.Delete(filePath);
        }
        catch(Exception exception)
        {
            Log.Debug(exception, "{LogContext} Missing-icon cache marker could not be inspected or removed. Path={Path}", LogContext, filePath);
        }

        return false;
    }

    private string GetCacheFilePath(string itemId, int size)
    {
        var fileName = $"{itemId}_{size}.png";

        return Path.Combine(_cacheDirectory, fileName);
    }

    private string GetMissingCacheFilePath(string itemId, int size)
    {
        var fileName = $"{itemId}_{size}.missing";

        return Path.Combine(_cacheDirectory, fileName);
    }

    private static string NormalizeItemId(string itemId)
    {
        const string prefix = "minecraft:";

        return itemId.StartsWith(prefix, StringComparison.Ordinal) ? itemId[prefix.Length..] : itemId;
    }

    private static Bitmap LoadImage(byte[] data)
    {
        using MemoryStream stream = new(data);
        using Bitmap source = new(stream);

        return new Bitmap(source);
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

    private sealed class MemoryCacheEntry
    {
        public required Bitmap Bitmap { get; init; }

        public required LinkedListNode<string> UsageNode { get; init; }
    }
}