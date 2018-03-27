namespace Sfa.Das.ApprenticeshipInfoService.Health
{
    using System;
    using System.Collections.Generic;

    public interface IHealthSettings
    {
        string Environment { get; }

        IEnumerable<Uri> ElasticsearchUrls { get; }

        IEnumerable<string> RequiredIndexAliases { get; }

        string LarsSiteRootUrl { get; }

        string LarsSiteDownloadsPageUrl { get; }

        string CourseDirectoryUrl { get; }
    }
}