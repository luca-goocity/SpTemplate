using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SpMediator.Models;

namespace Common.Libs.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            
            var originalBody = context.Response.Body;

            try
            {
                using var memStream = new MemoryStream();
                context.Response.Body = memStream;

                await next(context);

                memStream.Position = 0;

                try
                {
                    await ReadJsonResponse(memStream);
                }
                catch
                {
                    _logger.LogWarning("Cannot handle response: Response was not in JSON format.");
                }

                memStream.Position = 0;

                await memStream.CopyToAsync(originalBody);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task ReadJsonResponse(Stream memStream)
    {
        var responseBody = await new StreamReader(memStream).ReadToEndAsync();
        
        var response = JsonSerializer.Deserialize<ErrorsVm>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (response is null) _logger.LogError("Null response");
        else if (response.HasErrors)
        {
            foreach (var error in response.Errors)
            {
                _logger.LogError(error);
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        LogException(context.Response.StatusCode, exception.Message);

        var result = JsonSerializer.Serialize(new ErrorsVm
        {
            Errors = new() { exception.Message },
            TotalCount = 1
        });

        await context.Response.WriteAsync(result);
    }

    private void LogException(int statusCode, string message)
    {
        if (statusCode == StatusCodes.Status500InternalServerError) _logger.LogCritical(statusCode, message);
        else _logger.LogError(statusCode, message);
    }
}