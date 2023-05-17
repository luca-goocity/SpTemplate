using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SpEndpoints.Abstractions;

namespace SpEndpoints.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointDefinitions(this IServiceCollection services, params Type[] scanMarkers)
    {
        services.Configure<JsonOptions>(opt =>
        {
            opt.SerializerOptions.PropertyNameCaseInsensitive = true;
        });
        
        var endpointDefinitions = new List<IEndpointDefinition>();

        foreach (var marker in scanMarkers)
        {
            endpointDefinitions.AddRange(
                marker.Assembly.ExportedTypes
                    .Where(xx => typeof(IEndpointDefinition).IsAssignableFrom(xx) 
                                 && !xx.IsAbstract 
                                 && !xx.IsInterface)
                    .Select(Activator.CreateInstance)
                    .Cast<IEndpointDefinition>()
                );
        }

        services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
        
        return services;
    }
}