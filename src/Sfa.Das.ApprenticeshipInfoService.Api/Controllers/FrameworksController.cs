﻿namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using Sfa.Das.ApprenticeshipInfoService.Api.Attributes;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using SFA.DAS.Apprenticeships.Api.Types;
    using Swashbuckle.Swagger.Annotations;

    public class FrameworksController : ApiController
    {
        private readonly IGetFrameworks _getFrameworks;

        public FrameworksController(IGetFrameworks getFrameworks)
        {
            _getFrameworks = getFrameworks;
        }

        // GET /frameworks
        [SwaggerOperation("GetAll")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(IEnumerable<FrameworkSummary>))]
        [Route("frameworks")]
        [ExceptionHandling]
        public IEnumerable<FrameworkSummary> Get()
        {
            var response = _getFrameworks.GetAllFrameworks().ToList();

            foreach (var item in response)
            {
                item.Uri = Resolve(item.Id);
            }

            return response;
        }

        // GET /frameworks/40338

        /// <summary>
        /// Get a framework by composite id
        /// </summary>
        /// <param name="id">{FrameworkId}-{ProgType}-{PathwayId} ie: 403-3-8</param>
        /// <returns>the requested Framework</returns>
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(Framework))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/{id}")]
        [ExceptionHandling]
        public Framework Get(string id)
        {
            var response = _getFrameworks.GetFrameworkById(id);

            if (response == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            response.Uri = Resolve(response.FrameworkId);
            return response;
        }

        // HEAD /frameworks/5

        /// <summary>
        /// check if a framework exists by composite id
        /// </summary>
        /// <param name="id">{FrameworkId}{ProgType}{PathwayId} ie: 40338</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/{id}")]
        [ExceptionHandling]
        public void Head(string id)
        {
            if (_getFrameworks.GetFrameworkById(id) != null)
            {
                return;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        private string Resolve(string id)
        {
            return Url.Link("DefaultApi", new { controller = "frameworks", id = id });
        }
    }
}