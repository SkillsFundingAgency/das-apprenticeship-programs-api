﻿using Sfa.Das.ApprenticeshipInfoService.Core.Helpers;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers
{
    public class ControllerHelper : IControllerHelper
    {
        public int GetActualPage(int page)
        {
            return page < 1 ? 1 : page;
        }

        public DetailProviderResponse CreateDetailProviderResponse(ApprenticeshipDetails model, IApprenticeshipProduct apprenticeshipProduct, ApprenticeshipTrainingType apprenticeshipProductType)
        {
            if (model == null || apprenticeshipProduct == null)
            {
                return new DetailProviderResponse
                {
                    StatusCode = DetailProviderResponse.ResponseCodes.ApprenticeshipProviderNotFound
                };
            }

            var response = new DetailProviderResponse
            {
                StatusCode = DetailProviderResponse.ResponseCodes.Success,
                ApprenticeshipDetails = model,
                ApprenticeshipType = apprenticeshipProductType,
                ApprenticeshipName = apprenticeshipProduct.Title,
                ApprenticeshipLevel = apprenticeshipProduct.Level.ToString()
            };

            return response;
        }
    }
}
