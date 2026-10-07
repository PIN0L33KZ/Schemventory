using Schemventory.App;

namespace Schemventory.Services;

public sealed class ItemIconService
{
    private static readonly HttpClient HttpClient = new();
    private readonly string _cacheDirectory;
    private readonly SemaphoreSlim _downloadSemaphore = new(6);

    public ItemIconService()
    {
        _cacheDirectory = Path.Combine(Constants.CacheDirectory, "ItemIcons");

        _ = Directory.CreateDirectory(_cacheDirectory);
    }

    public async Task<Image> GetItemIconAsync(string itemId, int size = 64, CancellationToken cancellationToken = default)
    {
        var normalizedItemId = NormalizeItemId(itemId);
        var cacheFilePath = GetCacheFilePath(normalizedItemId, size);

        if(File.Exists(cacheFilePath))
            return LoadImage(cacheFilePath);

        var imageData = await DownloadIconAsync(normalizedItemId, size, cancellationToken);

        if(imageData is null)
            return new Bitmap(Properties.Resources.MissingIcon);

        await File.WriteAllBytesAsync(cacheFilePath, imageData, cancellationToken);

        return LoadImage(cacheFilePath);
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

        using HttpResponseMessage response = await HttpClient.GetAsync(url, cancellationToken);

        if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        _ = response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    private string GetCacheFilePath(string itemId, int size)
    {
        var fileName = $"{itemId}_{size}.png";

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
}