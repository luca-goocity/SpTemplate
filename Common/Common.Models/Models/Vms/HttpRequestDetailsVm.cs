using Microsoft.AspNetCore.Http;

namespace Common.Models.Models.Vms;

public sealed record HttpRequestDetailsVm
{
    public required string ApiPath { get; init; }
    public string? Body { get; init; }
    public string? Ip { get; init; }
    public required string Sender { get; init; }
    public required IHeaderDictionary Headers { get; init; }
}