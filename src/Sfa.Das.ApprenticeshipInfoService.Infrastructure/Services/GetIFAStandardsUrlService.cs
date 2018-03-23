﻿using System;
using System.Net;
using Newtonsoft.Json;
using Sfa.Das.ApprenticeshipInfoService.Core.Configuration;
using Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models;
using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services
{
    public class GetIfaStandardsUrlService : IGetIfaStandardsUrlService
    {
        private readonly ILog _logger;
        private readonly IConfigurationSettings _applicationSettings;

        public GetIfaStandardsUrlService(ILog logger, IConfigurationSettings applicationSettings)
        {
            _logger = logger;
            _applicationSettings = applicationSettings;
        }

        public string GetStandardUri(string standardId)
        {
            using (var wc = new WebClient())
            {
                var ifaUrlBase = _applicationSettings.IfaStandardApiUrl;
                var urlToCall = $"{ifaUrlBase}/{standardId}";
                var url = string.Empty;
                try
                {
                    var json = wc.DownloadString(urlToCall);
                    var jsonResult = JsonConvert.DeserializeObject<IfaApiStandard>(json);
                    url = jsonResult.StandardPageUri;
                    return url;
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, $"IFA Url [{urlToCall}] failed to return details");
                    return url;
                }
            }
        }
    }
}
