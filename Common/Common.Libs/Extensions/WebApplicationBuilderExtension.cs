using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Common.Libs.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Host
            .UseSerilog((_, config) =>
            {
                config.ReadFrom.Configuration(builder.Configuration);
            });

        return builder;
    }

    public static T ConfigureAppSetting<T>(this WebApplicationBuilder builder)
        where T : class
    {
        var section = builder.Configuration.GetSection(typeof(T).Name);
        var setting = section.Get<T>();
        builder.Services.Configure<T>(section);
        if (setting is null) throw new NullReferenceException(nameof(setting));
        return setting;
    }
}