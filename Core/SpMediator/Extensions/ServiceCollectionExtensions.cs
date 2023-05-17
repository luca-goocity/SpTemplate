using Microsoft.Extensions.DependencyInjection;

namespace SpMediator.Extensions;

public static class ServiceCollectionExtensions
{
    
    public static IServiceCollection ConfigureSpMediator(this IServiceCollection services, Type type)
    {
        return services.AddMediatR(xx =>
        {
            xx.RegisterServicesFromAssembly(type.Assembly);
        });
    }
}