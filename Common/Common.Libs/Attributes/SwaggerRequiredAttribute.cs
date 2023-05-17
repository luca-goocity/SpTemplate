using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Libs.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SwaggerRequiredAttribute : Attribute
{

}

public class SwaggerRequiredFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var properties = context.Type.GetProperties();
        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute(typeof(SwaggerRequiredAttribute));
            var propertyNameInCamelCase = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
            
            if (attribute is null)
            {
                schema.Required.Remove(propertyNameInCamelCase);
                continue;
            }

            if (schema.Required is null)
            {
                schema.Required = new List<string>
                {
                    propertyNameInCamelCase
                }.ToHashSet();

                continue;
            }

            schema.Required.Add(propertyNameInCamelCase);
        }
    }
}