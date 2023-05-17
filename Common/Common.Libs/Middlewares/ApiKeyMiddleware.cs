using System.Text;
using System.Text.Json;
using Common.Models.Configs;
using Common.Models.Models.Constants;
using Common.Models.Models.Vms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Libs.Middlewares;

public sealed class ApiKeyMiddleware : IMiddleware
{
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private readonly ApiKeyConfig _apiKeyConfiguration;

    public ApiKeyMiddleware(ILogger<ApiKeyMiddleware> logger,
        IOptions<ApiKeyConfig> apiKeyConfiguration)
    {
        _logger = logger;
        _apiKeyConfiguration = apiKeyConfiguration.Value;
    }
    
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_apiKeyConfiguration.IsEnabled) return next(context);

        var apiPath = context.Request.Path.Value ?? string.Empty;
        var hosts = new[]
        {
            HostsPagesToSkip.Welcome.ToLower(),
            HostsPagesToSkip.Root.ToLower(),
            HostsPagesToSkip.FavIcon.ToLower(),
        };
            
        if (hosts.Contains(apiPath.ToLower())) return next(context);

        if (!context.Request.Headers.ContainsKey(ApiKey.Key))
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var body = string.Empty;
            if (context.Request.Method == HttpMethod.Post.Method)
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
                body = reader.ReadToEndAsync().Result.Replace("\n", string.Empty);
            }

            context.Request.Headers.TryGetValue("Origin", out var sender);
            var requestDetails = new HttpRequestDetailsVm
            {
                ApiPath = apiPath,
                Body = body,
                Headers = context.Request.Headers,
                Ip = context.Connection.RemoteIpAddress?.ToString(),
                Sender = sender.ToString().Trim().ToUpper()
            };

            var loggerPaylod = JsonSerializer.Serialize(requestDetails, jsonSerializerOptions);

            var exception = new Exception("Necessary HTTP headers not present!");
            _logger.Log(LogLevel.Error, $"{loggerPaylod}\n\n\n\n\n");

            throw exception;
        }

        var header = context.Request.Headers
            .FirstOrDefault(xx => xx.Key == ApiKey.Key);
        if (header.Value != ApiKey.Value) throw new Exception("Necessary HTTP headers not present!");

        return next(context);
    }
}