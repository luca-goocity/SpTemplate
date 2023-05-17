using Microsoft.Extensions.Logging;
using SpMediator.Handlers;
using SpTemplate.Business.First.Models.Reponses.Queries;
using SpTemplate.Business.First.Models.Requests.Queries;

namespace SpTemplate.Business.First.Handlers.Queries;

public sealed class HelloRequestHandler : BaseHandler<HelloRequest, HelloResponse>
{
    public HelloRequestHandler(ILogger<HelloRequestHandler> logger) : base(logger)
    {
        // D.I.
    }
    
    public override Task<HelloResponse> Handle(HelloRequest req, CancellationToken res)
    {
        _logger.LogInformation($"Start {nameof(HelloRequestHandler)}");

        var result = $"Ciao {req.Name}";
        
        _logger.LogInformation($"End {nameof(HelloRequestHandler)}");

        return Task.FromResult(new HelloResponse
        {
            Data = result,
            TotalCount = 1
        });
    }
}