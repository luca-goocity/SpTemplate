using MediatR;

namespace SpMediator.Models;

public record BaseRequest<TResponse> : IRequest<TResponse>;