namespace Common.Models.Configs;

public sealed record RedisConfig
{
    public required string ConnectionString { get; init; }
    public required int Database { get; init; }
    public required string Port { get; init; }
}