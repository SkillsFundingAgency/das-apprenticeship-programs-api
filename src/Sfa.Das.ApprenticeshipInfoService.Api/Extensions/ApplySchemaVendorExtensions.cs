using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swashbuckle.Swagger;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Extensions
{
    internal class ApplySchemaVendorExtensions : Swashbuckle.Swagger.ISchemaFilter
    {
        public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                     .Where(p => p.GetCustomAttributes(typeof(ObsoleteAttribute), true)?.Any() == true))
                if (schema?.properties?.ContainsKey(prop.Name) == true)
                    schema?.properties?.Remove(prop.Name);
        }
    }
}