﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http.Routing;
using Sfa.Das.ApprenticeshipInfoService.Api.Helpers;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    using System.Net;
    using System.Web.Http;
    using Sfa.Das.ApprenticeshipInfoService.Core.Models;
    using Sfa.Das.ApprenticeshipInfoService.Core.Services;
    using Swashbuckle.Swagger.Annotations;

    public class StandardController : ApiController
    {
        private readonly IGetStandards _getStandards;
        private readonly IControllerHelper _controllerHelper;

        public StandardController(IGetStandards getStandards,
            IControllerHelper controllerHelper)
        {
            _getStandards = getStandards;
            _controllerHelper = controllerHelper;
        }

        // GET api/values?take&page
        [SwaggerOperation("GetAll")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<StandardSummary> Get()
        {
            var response = _getStandards.GetAllStandards().ToList();

            foreach (var item in response)
            {
                item.Uri = Resolve(item.Id);
            }

            return response;
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public Standard GetStandard(int id)
        {
            var standard = _getStandards.GetStandardById(id);
            if (standard != null)
            {
                return standard;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        // HEAD api/values/5
        public void Head(int id)
        {
            if (_getStandards.GetStandardById(id) != null)
            {
                return;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        private string Resolve(int id)
        {
            return Url.Link("DefaultApi", new { controller = "Standard", id = id });
        }
    }
}
