using Microsoft.AspNetCore.Builder;

namespace SpEndpoints.Abstractions;

public interface IEndpointDefinition
{
    void MapEndpoints(WebApplication app);
}