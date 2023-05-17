using Common.Models.Configs;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Common.Cache.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, RedisConfig redisConfig)
    {
        services.AddOutputCache();

        services.RemoveAll<IOutputCacheStore>();
        
        // services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect($"{redisConfig.ConnectionString}:{redisConfig.Port}"));
        services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
        
        return services;
    }
}