﻿using SFA.DAS.NLog.Logger;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Logging
{
    using Newtonsoft.Json;
    
    public class ElasticSearchLogEntry : ILogEntry
    {
        public string Identifier { get; set; }

        public int? ReturnCode { get; set; }

        public bool? Successful { get; set; }

        public int SearchTime { get; set; }

        public double RequestTime { get; set; }

        public double MaxScore { get; set; }

        public int? HitCount { get; set; }

        public string Url { get; set; }

        public string Body { get; set; }

    }
}
