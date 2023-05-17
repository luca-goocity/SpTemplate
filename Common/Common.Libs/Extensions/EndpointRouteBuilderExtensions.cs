using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Common.Libs.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteGroupBuilder CreateGroup(this IEndpointRouteBuilder builder,
        string className,
        params string[]? suffixes)
    {
        className = className
            .Replace("Endpoints", string.Empty)
            .ToLower();

        var prefix = $"api/{className}";

        if (suffixes is not null)
        {
            suffixes = suffixes.Where(xx => !string.IsNullOrWhiteSpace(xx)).ToArray();
            foreach (var item in suffixes)
            {
                var suffix = item.ToLower();
                className = $"{className} {suffix}";
                var parts = suffix.Split(" ").AsSpan();
                foreach (var part in parts)
                {
                    prefix += $"/{part}";
                }
            }
        }
        
        return builder.MapGroup(prefix)
            .WithTags(className);
    }
}