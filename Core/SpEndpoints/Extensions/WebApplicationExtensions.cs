using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpEndpoints.Abstractions;

namespace SpEndpoints.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapEndpointDefinitions(this WebApplication app)
    {
        var endpointsDefinitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();
        
        foreach (var definition in endpointsDefinitions)
        {
            definition.MapEndpoints(app);
        }
        
        return app;
    }
}