using System.Buffers;
using Microsoft.Extensions.Caching.Distributed;

namespace VerticalSliceTest.Orders.Api.Infrastructure.Caching;

internal sealed class CacheService(IDistributedCache cache) : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);
        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = expiration,
        };

        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key, cancellationToken);
    }

    private static T? Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes, SerializerOptions);
    }

    private static byte[] Serialize<T>(T value)
    {
        ArrayBufferWriter<byte> buffer = new();

        using Utf8JsonWriter writer = new(buffer);
        JsonSerializer.Serialize(writer, value, SerializerOptions);

        return buffer.WrittenSpan.ToArray();
    }
}
