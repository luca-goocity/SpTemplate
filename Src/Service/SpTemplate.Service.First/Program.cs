using Common.Libs.Extensions;
using Common.Libs.Middlewares;
using Common.Libs.Utils;
using Common.Models.Configs;
using Microsoft.IdentityModel.Logging;
using SpEndpoints.Extensions;
using SpTemplate.Business.First.Extensions;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppSetting<ApiKeyConfig>();
var appConfig = builder.ConfigureAppSetting<AppConfig>();

builder.ConfigureSerilog();

builder.Services
    .RegisterServices()
    .RegisterFirstServices()
    .AddEndpointsApiExplorer()
    .AddCors()
    .AddSwaggerGen(SwaggerUtils.SetupSwaggerGen)
    .AddJwtAuth()
    .AddHealthChecks();

builder.Services.AddAuthentication();
builder.Services.AddRouting();
builder.Services.ConfigurePolicies();

var connectionString = builder.Configuration.GetConnectionString("MongoDb")!;

builder.Services
    .ConfigureCors()
    .AddResponseCompression()
    .AddMongoDb(connectionString, true)
    .AddRepositories()
    .AddFirstValidators()
    .AddMiddlewares()
    .AddMediatrBehaviors()
    .ConfigureFirstMediator()
    .AddFirstUtils();

builder.Services.AddEndpointDefinitions(typeof(Program));

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger()
        .UseSwaggerUI(SwaggerUtils.SetupSwaggerUi) // localhost:xxxx/swagger/index.html
        .ConfigureRedoc(); // localhost:xxxx/api-docs/index.html
}

app.MapHealthChecks("/HealthCheck");

app.UseCors("AllowOrigins")
    .UseResponseCompression()
    .UseMiddleware<ExceptionHandlingMiddleware>()
    .UseMiddleware<ApiKeyMiddleware>()
    .UseAuthentication()
    .UseRouting()
    .UseAuthorization();

if (appConfig.UseCache) app.UseOutputCache();

app.UseWelcomePage("/");

app.MapEndpointDefinitions();

app.Run();