using Common.Libs.Extensions;
using Common.Models.Constants;
using MediatR;
using SpTemplate.Business.First.Models.Requests.Queries;
using SpTemplate.Service.First.Endpoints.Abstraction;

namespace SpTemplate.Service.First.Endpoints.Implementation;

public sealed class HelloEndpoints : IHelloEndpoints
{
    public void MapEndpoints(WebApplication app)
    {
        var group = app.CreateGroup(nameof(HelloEndpoints));

        group.MapGet(CommonRoutes.Root, Get)
            .WithCustomConfiguration<int>(nameof(Get), "description")
            .AllowAnonymous();
    }

    public async Task<IResult> Get(IMediator mediator,
        string? name,
        CancellationToken ct)
    {
        var req = new HelloRequest
        {
            Name = name ?? string.Empty
        };

        var res = await mediator.Send(req, ct);

        return res.HasErrors 
            ? Results.BadRequest(res) 
            : Results.Ok(res);
    }
}