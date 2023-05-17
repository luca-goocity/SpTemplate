using MediatR;
using Microsoft.Extensions.Logging;

namespace SpMediator.Handlers;

public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly ILogger<BaseHandler<TRequest, TResponse>> _logger;

    protected BaseHandler(ILogger<BaseHandler<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public abstract Task<TResponse> Handle(TRequest req, CancellationToken ct);
}