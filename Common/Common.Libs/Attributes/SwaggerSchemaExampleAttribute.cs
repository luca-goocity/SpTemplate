using System.Collections;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Libs.Attributes;

[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Parameter |
    AttributeTargets.Property |
    AttributeTargets.Enum)]
public class SwaggerSchemaExampleAttribute : Attribute
{
    public SwaggerSchemaExampleAttribute(object? example)
    {
        Example = example;
    }

    public object? Example { get; set; }
}

public class SwaggerSchemaExampleFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var schemaAttribute = context.MemberInfo
            ?.GetCustomAttributes<SwaggerSchemaExampleAttribute>()
            .FirstOrDefault();

        if (schemaAttribute is not null) ApplySchemaAttribute(schema, schemaAttribute);
    }

    private void ApplySchemaAttribute(OpenApiSchema schema, SwaggerSchemaExampleAttribute schemaAttribute)
    {
        schema.Example = schemaAttribute.Example switch
        {
            string value => new OpenApiString(value),
            int value => new OpenApiInteger(value),
            IEnumerable<byte> value => new OpenApiBinary(value.ToArray()),
            IEnumerable _ => new OpenApiArray(),
            bool value => new OpenApiBoolean(value),
            byte value => new OpenApiByte(value),
            DateTime value => new OpenApiDateTime(value),
            DateOnly value => new OpenApiDate(value.ToDateTime(TimeOnly.MinValue)),
            double value => new OpenApiDouble(value),
            float value => new OpenApiFloat(value),
            long value => new OpenApiLong(value),
            null => new OpenApiNull(),
            _ => new OpenApiObject(),
        };
    }
}