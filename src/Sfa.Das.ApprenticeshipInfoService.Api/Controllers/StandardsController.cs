using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Das.ApprenticeshipInfoService.Core.Services;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]   
    public class StandardsController : ControllerBase
    {
        private readonly IGetStandards _getStandards;
        private readonly ILogger<StandardsController> _logger;

        public StandardsController(IGetStandards getStandards, ILogger<StandardsController> logger)
        {
            _getStandards = getStandards;
            _logger = logger;
        }

        /// <summary>
        /// Get all the active standards
        /// </summary>
        /// <returns>a collection of standards</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/standards", Name="GetAllActiveStandards")]
        public ActionResult<IEnumerable<StandardSummary>> Get()
        {
            try
            {
                var response = _getStandards.GetAllStandards().Where(s => s.IsActiveStandard);

                foreach (var item in response)
                {
                    item.Uri = Resolve(item.Id);
                }

                return response.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "/standards");
                throw;
            }
        }

        /// <summary>
        /// Get all standards
        /// </summary>
        /// <returns>a collection of standards</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("/standards/v2", Name="GetAllStandards")]
        public ActionResult<IEnumerable<StandardSummary>> GetAll()
        {
            try
            {
                var response = _getStandards.GetAllStandards();

                foreach (var item in response)
                {
                    item.Uri = Resolve(item.Id);
                }

                return response.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "/standards");
                throw;
            }
        }

        /// <summary>
        /// Get a standard
        /// </summary>
        /// <param name="id">{standardid}</param>
        /// <returns>a standard</returns>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("/standards/{id}", Name="GetStandardById")]
        public ActionResult<Standard> Get(string id)
        {
            var standard = _getStandards.GetStandardById(id);

            if (standard == null)
            {
                return NotFound();
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("/standards/getlistbyids", Name="GetStandardsById")]
        public ActionResult<List<Standard>> Get(string ids, int page = 1)
        {
            var listIds = ValidateIds(ids);

            var standards = _getStandards.GetStandardsById(listIds, page);

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
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/standards", Name="StandardssExists")]
        public void Head()
        {
            Get();
        }

        /// <summary>
        /// Standard exists?
        /// </summary>
        /// <param name="id">{standardid}</param>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpHead("/standards/{id}", Name="StandardsExistsById")]
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
                    throw new Exception($"{idElement} is not a valid id");
                }
            }

            return response;
        }

        private string Resolve(string id)
        {
            return Url.Link("GetStandardById", new { id = id });
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
