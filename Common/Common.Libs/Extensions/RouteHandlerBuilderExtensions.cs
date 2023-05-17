using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Libs.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithCustomConfiguration<TResponse>(
        this RouteHandlerBuilder builder,
        string displayName,
        string description)
    {
        builder
            .WithOpenApi(opt =>
            {
                opt.Summary = displayName;
                opt.Description = description;
        
                return opt;
            })
            .Produces<TResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        return builder;
    }
}