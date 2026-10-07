using Schemventory.App;
using System.Collections.Concurrent;
using System.Net;

namespace Schemventory.Services;

public sealed class ItemIconService
{
    private static readonly TimeSpan MissingCacheLifetime = TimeSpan.FromDays(7);

    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly string _cacheDirectory;
    private readonly SemaphoreSlim _downloadSemaphore = new(6);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _itemLocks = new(StringComparer.Ordinal);

    public ItemIconService()
    {
        _cacheDirectory = Path.Combine(Constants.CacheDirectory, "ItemIcons");

        _ = Directory.CreateDirectory(_cacheDirectory);
    }

    public async Task<Image> GetItemIconAsync(
        string itemId,
        int size = 64,
        CancellationToken cancellationToken = default)
    {
        var normalizedItemId = NormalizeItemId(itemId);
        var cacheFilePath = GetCacheFilePath(normalizedItemId, size);
        var missingCacheFilePath = GetMissingCacheFilePath(normalizedItemId, size);

        var cachedImage = TryLoadCachedImage(cacheFilePath);

        if(cachedImage is not null)
            return cachedImage;

        if(IsMissingCacheValid(missingCacheFilePath))
            return new Bitmap(Properties.Resources.MissingIcon);

        var cacheKey = $"{normalizedItemId}:{size}";
        var itemLock = _itemLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

        await itemLock.WaitAsync(cancellationToken);

        try
        {
            // Another caller may have filled the cache while this one was waiting.
            cachedImage = TryLoadCachedImage(cacheFilePath);

            if(cachedImage is not null)
                return cachedImage;

            if(IsMissingCacheValid(missingCacheFilePath))
                return new Bitmap(Properties.Resources.MissingIcon);

            var imageData = await DownloadIconAsync(normalizedItemId, size, cancellationToken);

            if(imageData is null)
            {
                await File.WriteAllTextAsync(missingCacheFilePath, string.Empty, cancellationToken);

                return new Bitmap(Properties.Resources.MissingIcon);
            }

            TryDeleteFile(missingCacheFilePath);

            await File.WriteAllBytesAsync(cacheFilePath, imageData, cancellationToken);

            return LoadImage(cacheFilePath);
        }
        finally
        {
            _ = itemLock.Release();
        }
    }

    private async Task<byte[]?> DownloadIconAsync(
        string itemId,
        int size,
        CancellationToken cancellationToken)
    {
        await _downloadSemaphore.WaitAsync(cancellationToken);

        try
        {
            var imageData = await TryDownloadIconAsync("item", itemId, size, cancellationToken);

            return imageData is not null
                ? imageData
                : await TryDownloadIconAsync("block", itemId, size, cancellationToken);
        }
        finally
        {
            _ = _downloadSemaphore.Release();
        }
    }

    private static async Task<byte[]?> TryDownloadIconAsync(
        string type,
        string itemId,
        int size,
        CancellationToken cancellationToken)
    {
        var url = $"https://blockrender.dev/render/{type}/{Uri.EscapeDataString(itemId)}.png?size={size}";

        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd("Schemventory/1.0");

        using HttpResponseMessage response = await HttpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if(response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if(response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new HttpRequestException("Item icon request was rate limited.", null, response.StatusCode);

        _ = response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    private Image? TryLoadCachedImage(string filePath)
    {
        if(!File.Exists(filePath))
            return null;

        try
        {
            return LoadImage(filePath);
        }
        catch
        {
            TryDeleteFile(filePath);
            return null;
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
        catch
        {
            // If the marker cannot be inspected or removed, allow a fresh request.
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

        return itemId.StartsWith(prefix, StringComparison.Ordinal)
            ? itemId[prefix.Length..]
            : itemId;
    }

    private static Image LoadImage(string filePath)
    {
        var data = File.ReadAllBytes(filePath);

        using MemoryStream stream = new(data);

        return new Bitmap(stream);
    }

    private static void TryDeleteFile(string filePath)
    {
        try
        {
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
        catch
        {
            // Cache cleanup must never prevent the application from working.
        }
    }
}
