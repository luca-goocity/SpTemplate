using SpMediator.Models;
using SpTemplate.Business.First.Models.Reponses.Queries;

namespace SpTemplate.Business.First.Models.Requests.Queries;

public sealed record HelloRequest : BaseRequest<HelloResponse>
{
    public required string Name { get; init; }
}