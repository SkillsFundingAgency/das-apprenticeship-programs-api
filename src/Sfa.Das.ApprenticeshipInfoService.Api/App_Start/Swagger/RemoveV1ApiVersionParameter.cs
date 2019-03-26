using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Swagger
{

    internal class RemoveV1ApiVersionParameter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (apiDescription.GetGroupName() == "v1" && operation.parameters != null)
            {
                var versionParameter = operation.parameters.SingleOrDefault(x => x.name == "api-version");

                if (versionParameter != null)
                {
                    operation.parameters.Remove(versionParameter);
                }
            }
        }
    }
}