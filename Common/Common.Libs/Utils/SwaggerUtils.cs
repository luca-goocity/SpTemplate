using System.ComponentModel;
using System.Reflection;
using Common.Libs.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Common.Libs.Utils;

public static class SwaggerUtils
{
    private const string SwaggerSpecUrl = "/swagger/v1/swagger.json";
    
    public static void SetupSwaggerGen(SwaggerGenOptions configuration)
    {
        configuration.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);

        configuration.CustomSchemaIds(type =>
        {
            // If present, show the [DisplayName]
            var displayName = GetDisplayName(type);
            if (!string.IsNullOrWhiteSpace(displayName)) return displayName;
            
            // If type is a generic, also show the custom types
            if (type.GenericTypeArguments.Any()) return FormatGenericCustomTypes(type);
            
            return type.Name;
        });

        configuration.SchemaFilter<SwaggerSchemaExampleFilter>();
        configuration.SchemaFilter<SwaggerRequiredFilter>();
        configuration.SchemaFilter<SwaggerExcludeFilter>();

        configuration.DescribeAllParametersInCamelCase();
    }

    public static void SetupSwaggerUi(SwaggerUIOptions options)
    {
        options.SwaggerEndpoint(SwaggerSpecUrl, "V1");
    }

    public static void ConfigureRedoc(this IApplicationBuilder app)
    {
        app.UseReDoc(options =>
        {
            options.DocumentTitle = "API Documentation";
            options.SpecUrl = SwaggerSpecUrl;
            options.RoutePrefix = "api-docs";
        });
    }

    private static string? GetDisplayName(ICustomAttributeProvider type)
    {
        return type.GetCustomAttributes(false)
            .OfType<DisplayNameAttribute>()
            .FirstOrDefault()
            ?.DisplayName;
    }

    private static string FormatGenericCustomTypes(Type type)
    {
        var customTypes = string.Join(",", type.GenericTypeArguments.Select(xx => xx.Name));
        var typeName = type.Name.Split("`")[0];
        return $"{typeName}<{customTypes}>";
    }
}