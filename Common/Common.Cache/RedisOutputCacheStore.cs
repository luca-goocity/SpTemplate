using Common.Models.Configs;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Common.Cache;

public sealed class RedisOutputCacheStore : IOutputCacheStore
{
    private readonly IDatabase _db;

    public RedisOutputCacheStore(IConnectionMultiplexer connectionMultiplexer, 
        IOptions<RedisConfig> redisConfig)
    {
        _db = connectionMultiplexer.GetDatabase(redisConfig.Value.Database);
    }

    public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        var cachedKeys = await _db.SetMembersAsync(tag);
        var keys = cachedKeys.Select(xx => (RedisKey)xx.ToString())
            .Concat(new[] { (RedisKey)tag })
            .ToArray();

        await _db.KeyDeleteAsync(keys);
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        return await _db.StringGetAsync(key);
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor,
        CancellationToken cancellationToken)
    {
        foreach (var tag in tags ?? Array.Empty<string>())
        {
            await _db.SetAddAsync(tag, key);
        }

        await _db.StringSetAsync(key, value, validFor);
    }
}