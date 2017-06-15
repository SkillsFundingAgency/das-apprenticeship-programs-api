﻿namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
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

    public class FrameworksController : ApiController
    {
        private readonly IGetFrameworks _getFrameworks;
        private readonly ILog _logger;

        public FrameworksController(
            IGetFrameworks getFrameworks,
            ILog logger)
        {
            _getFrameworks = getFrameworks;
            _logger = logger;
        }

        /// <summary>
        /// Get all the active frameworks
        /// </summary>
        /// <returns>a collection of frameworks</returns>
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

        /// <summary>
        /// Get a framework
        /// </summary>
        /// <param name="id">{FrameworkId}-{ProgType}-{PathwayId}</param>
        /// <returns>a framework</returns>
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

        /// <summary>
        /// Do we have frameworks?
        /// </summary>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks")]
        [ExceptionHandling]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// framework exists?
        /// </summary>
        /// <param name="id">{FrameworkId}-{ProgType}-{PathwayId}</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("frameworks/{id}")]
        [ExceptionHandling]
        public void Head(string id)
        {
            Get(id);
        }

        private string Resolve(string id)
        {
            return Url.Link("DefaultApi", new { controller = "frameworks", id = id });
        }
    }
}
