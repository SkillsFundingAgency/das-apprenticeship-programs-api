using System;
using System.Linq;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public static class ITrainingProgrammeExtensions
    {
        public static bool IsActiveOn(this ITrainingProgramme course, DateTime date)
        {
            return GetStatusOn(course.EffectiveFrom, course.EffectiveTo, date) == TrainingProgrammeStatus.Active;
        }

        public static TrainingProgrammeStatus GetStatusOn(this ITrainingProgramme course, DateTime date)
        {
            return GetStatusOn(course.EffectiveFrom, course.EffectiveTo, date);
        }

        public static int FundingCapOn(this ITrainingProgramme course, DateTime date)
        {
            if (!course.IsActiveOn(date))
                return 0;

            var applicableFundingPeriod = course.FundingPeriods.FirstOrDefault(x => GetStatusOn(x.EffectiveFrom, x.EffectiveTo, date) == TrainingProgrammeStatus.Active);

            return applicableFundingPeriod?.FundingCap ?? 0;
        }


        private static TrainingProgrammeStatus GetStatusOn(DateTime? effectiveFrom, DateTime? effectiveTo, DateTime date)
        {
            var dateOnly = date.Date;

            if (effectiveFrom.HasValue && effectiveFrom.Value.FirstOfMonth() > dateOnly)
                return TrainingProgrammeStatus.Pending;

            if (!effectiveTo.HasValue || effectiveTo.Value >= dateOnly)
                return TrainingProgrammeStatus.Active;

            return TrainingProgrammeStatus.Expired;
        }

        private static DateTime FirstOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }
    }
}