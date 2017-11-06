using System;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Helpers
{
    public class DateHelper
    {
        public static bool CheckEffectiveDates(DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            var effectiveFromInThePast = effectiveFrom.HasValue && effectiveFrom.Value <= DateTime.Now;
            var effectiveToIntheFuture = !effectiveTo.HasValue || effectiveTo.Value > DateTime.Now;

            return effectiveFromInThePast && effectiveToIntheFuture;
        }
    }
}
