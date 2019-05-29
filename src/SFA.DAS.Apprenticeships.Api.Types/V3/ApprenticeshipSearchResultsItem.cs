using System;
using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types.V3
{
	public class ApprenticeshipSearchResultsItem
	{
        private string _title { get; set; }
		public string Id { get; set; }
        public ApprenticeshipTrainingType ProgrammeType { get; set; }
		public string Title
        {
            get { return ProgrammeType == ApprenticeshipTrainingType.Framework && HasSubGroups == false ? FrameworkName : _title; }
            set { _title = value; }
        }
		public List<string> JobRoles { get; set; }
		public List<string> Keywords { get; set; }
		public int Level { get; set; }
		public bool Published { get; set; }
		public int Duration { get; set; }
		public DateTime? EffectiveFrom { get; set; }
		public DateTime? EffectiveTo { get; set; }
        public DateTime? LastDateForNewStarts { get; set; }

        // Should be removed when frameworks are finally gone.
        public string FrameworkName { get; set; }
        public string PathwayName { get; set; }
        private bool HasSubGroups => !string.Equals(FrameworkName.Trim(), PathwayName.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
