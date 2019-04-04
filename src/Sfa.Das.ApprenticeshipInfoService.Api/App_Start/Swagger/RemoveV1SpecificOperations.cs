//using System.Linq;
//using System.Web.Http.Description;
//using Swashbuckle.Swagger;

//namespace Sfa.Das.ApprenticeshipInfoService.Api.Swagger
//{
//    internal class RemoveV1SpecificOperations : IDocumentFilter
//    {
//        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
//        {
//            var v1Paths = swaggerDoc.paths.Where(x => x.Key.StartsWith("/v1/")).ToList();

//            foreach (var path in v1Paths)
//            {
//                swaggerDoc.paths.Remove(path.Key);
//            }
//        }
//    }
//}