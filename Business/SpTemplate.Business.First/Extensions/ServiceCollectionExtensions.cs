using Common.Libs.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SpMediator.Extensions;

namespace SpTemplate.Business.First.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureFirstMediator(this IServiceCollection services)
    {
        return services.ConfigureSpMediator(typeof(ServiceCollectionExtensions));
    }
    
    public static IServiceCollection AddFirstValidators(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssembly(typeof(ServicesCollectionExtensions).Assembly,
            ServiceLifetime.Transient,
            filter: null,
            includeInternalTypes: true);
    }
    
    public static IServiceCollection RegisterFirstServices(this IServiceCollection services)
    {
        // services.AddTransient<IService, Service>();

        return services;
    }

    public static IServiceCollection AddFirstUtils(this IServiceCollection services)
    {
        return services
                // .AddTransient<IUtils, Utils>()
            ;
    }
}