namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types;
    using SFA.DAS.NLog.Logger;
    using Swashbuckle.Swagger.Annotations;

    public class StandardsController : ApiController
    {
        private readonly IGetStandards _getStandards;
        private readonly ILog _logger;

        public StandardsController(IGetStandards getStandards, ILog logger)
        {
            _getStandards = getStandards;
            _logger = logger;
        }

        /// <summary>
        /// Get all the active standards
        /// </summary>
        /// <returns>a collection of standards</returns>
        [SwaggerOperation("GetAllActiveStandards")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<StandardSummary>))]
        [Route("standards")]
        [ExceptionHandling]
        public IEnumerable<StandardSummary> Get()
        {
            try
            {
                var response = _getStandards.GetAllStandards().Where(s => s.IsActiveStandard);

                foreach (var item in response)
                {
                    item.Uri = Resolve(item.Id);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e, "/standards");
                throw;
            }
        }

        /// <summary>
        /// Get all standards
        /// </summary>
        /// <returns>a collection of standards</returns>
        [SwaggerOperation("GetAllStandards")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<StandardSummary>))]
        [Route("standards/v2")]
        [ExceptionHandling]
        public IEnumerable<StandardSummary> GetAll()
        {
            try
            {
                var response = _getStandards.GetAllStandards();

                foreach (var item in response)
                {
                    item.Uri = Resolve(item.Id);
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Error(e, "/standards");
                throw;
            }
        }

        /// <summary>
        /// Get a standard
        /// </summary>
        /// <param name="id">{standardid}</param>
        /// <returns>a standard</returns>
        [SwaggerOperation("GetStandardById")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Standard))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("standards/{id}")]
        [ExceptionHandling]
        public Standard Get(string id)
        {
            var standard = _getStandards.GetStandardById(id);

            if (standard == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            standard.Uri = Resolve(standard.StandardId);
            standard.Providers = ResolveProvidersUrl(id);
            return standard;
        }

        /// <summary>
        /// Get a list of standards
        /// </summary>
        /// <param name="ids">Standard ids, coma separated</param>
        /// <param name="page">Page you want to get</param>
        /// <returns>a list of standard</returns>
        [SwaggerOperation("GetStandardsById")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(List<Standard>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("standards/getlistbyids")]
        [ExceptionHandling]
        public List<Standard> Get(string ids, int page = 1)
        {
            var listIds = ValidateIds(ids);

            var standards = _getStandards.GetStandardsById(listIds);

            foreach (var standard in standards)
            {
                standard.Uri = Resolve(standard.StandardId);
                standard.Providers = ResolveProvidersUrl(standard.StandardId);
            }

            return standards;
        }

        /// <summary>
        /// Do we have standards?
        /// </summary>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("standards")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// Standard exists?
        /// </summary>
        /// <param name="id">{standardid}</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("standards/{id}")]
        [ExceptionHandling]
        public void Head(string id)
        {
            Get(id);
        }

        private List<int> ValidateIds(string ids)
        {
            var response = new List<int>();
            foreach (var idElement in ids.Split(','))
            {
                int id = 0;
                var validInt = int.TryParse(idElement, out id);

                if (validInt)
                {
                    response.Add(id);
                }
                else
                {
                    throw new Exception("At least one is not a valid id");
                }
            }

            return response;
        }

        private string Resolve(string id)
        {
            return Url.Link("DefaultApi", new { controller = "standards", id = id });
        }

        private ProvidersHref ResolveProvidersUrl(string id)
        {
            return new ProvidersHref
            {
                Href = Url.Link("GetStandardProviders", new { standardId = id })
            };
        }
    }
}
