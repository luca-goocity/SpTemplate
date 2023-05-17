namespace Common.Models.Configs;

public sealed record AppConfig
{
    public required bool UseCache { get; init; }
}