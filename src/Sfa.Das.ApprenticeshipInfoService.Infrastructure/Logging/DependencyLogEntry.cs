namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Logging
{
    public class DependencyLogEntry
    {
        public string Identifier { get; set; }

        public double ResponseTime { get; set; }

        public int? ResponseCode { get; set; }

        public string Url { get; set; }
    }
}
