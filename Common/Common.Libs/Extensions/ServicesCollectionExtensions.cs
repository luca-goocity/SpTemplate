using System.Diagnostics;
using Common.Identity.Models;
using Common.Identity.Policies.Handlers;
using Common.Identity.Policies.Requirements;
using Common.Libs.Behaviors;
using Common.Libs.Middlewares;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using SpRepo.Abstraction;
using SpRepo.Implementation;

namespace Common.Libs.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }

    public static IServiceCollection ConfigurePolicies(this IServiceCollection services)
    {
        services.AddCustomPolicyHandler<CanDeletePolicyHandler>()
            .AddCustomPolicyHandler<CanAddPolicyHandler>()
            .AddCustomPolicyHandler<CanEditPolicyHandler>()
            .AddCustomPolicyHandler<ScopePolicyHandler>()
            ;
        // Register here the new handler like ↑

        return services.AddAuthorization(options =>
        {
            options.AddCustomPolicy<CanDeletePolicyRequirement>(Policies.CanDelete)
                .AddCustomPolicy<CanAddPolicyRequirement>(Policies.CanAdd)
                .AddCustomPolicy<CanEditPolicyRequirement>(Policies.CanEdit)
                .AddCustomPolicy<ScopePolicyRequirement>(Policies.Scope)
                ;
            // Register here the new policy like ↑
            // You also have to register the new handler on top of this method
        });
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5001";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

        return services;
    }

    public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString,
        bool logQueryOnConsole = false)
    {
        services.AddSingleton<IMongoClient>(_ =>
        {
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var settings = MongoClientSettings.FromUrl(mongoUrlBuilder.ToMongoUrl());

            if (logQueryOnConsole)
            {
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e => { Debug.WriteLine($"{e.CommandName} - {e.Command}"); });
                };
            }

            return new MongoClient(settings);
        });

        services.AddScoped<IClientSessionHandle>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            return client.StartSession();
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();

            var databaseName = MongoUrl.Create(connectionString).DatabaseName;

            return client.GetDatabase(databaseName);
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }

    public static IServiceCollection AddMediatrBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }

    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
        services.AddTransient<ApiKeyMiddleware>();
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services
            .AddCors(action => action
                .AddPolicy("AllowOrigins", options => options
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                ));

        return services;
    }

    private static IServiceCollection AddCustomPolicyHandler<T>(this IServiceCollection services)
        where T : class, IAuthorizationHandler
    {
        return services.AddTransient<IAuthorizationHandler, T>();
    }

    private static AuthorizationOptions AddCustomPolicy<T>(this AuthorizationOptions options, string name)
        where T : IAuthorizationRequirement, new()
    {
        options.AddPolicy(name, policy => policy.Requirements.Add(new T()));
        return options;
    }
}