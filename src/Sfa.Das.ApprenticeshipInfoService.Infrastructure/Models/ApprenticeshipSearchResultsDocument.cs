using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Models
{
	public class ApprenticeshipSearchResultsDocument
	{
		public string Title { get; set; }

		// Standards
		public int? StandardId { get; set; }

		public List<string> JobRoles { get; set; }

		public List<string> Keywords { get; set; }

		// Frameworks
		public string FrameworkId { get; set; }

		public string FrameworkName { get; set; }

		public string PathwayName { get; set; }

		public int Level { get; set; }

		public IEnumerable<JobRoleItem> JobRoleItems { get; set; }

		public bool Published { get; set; }

		public int Duration { get; set; }

		public string TitleKeyword { get; set; }

		public DateTime? EffectiveFrom { get; set; }

		public DateTime? EffectiveTo { get; set; }

        public DateTime? LastDateForNewStarts { get; set; }
    }
}
