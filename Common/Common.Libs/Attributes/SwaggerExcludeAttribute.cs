using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Libs.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SwaggerExcludeAttribute : Attribute
    {
    }

    public class SwaggerExcludeFilter : ISchemaFilter
    {
        #region Public Methods

        public void Apply(OpenApiSchema schema, SchemaFilterContext? context)
        {
            if (schema?.Properties == null || context == null) return;
            var excludedProperties = context.Type.GetProperties()
                .Where(t => t.GetCustomAttribute(typeof(SwaggerExcludeAttribute), true) != null);
            foreach (var excludedProperty in excludedProperties)
            {
                var propertyNameInCamelCase = char.ToLowerInvariant(excludedProperty.Name[0]) + excludedProperty.Name[1..];
                if (schema.Properties.ContainsKey(propertyNameInCamelCase))
                    schema.Properties.Remove(propertyNameInCamelCase);
            }
        }

        #endregion Public Methods
    }
}