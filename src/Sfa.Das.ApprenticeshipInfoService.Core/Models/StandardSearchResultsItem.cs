﻿namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
    using System.Collections.Generic;
    using SFA.DAS.Apprenticeships.Api.Types;

    public sealed class StandardSearchResultsItem
    {
        public string StandardId { get; set; }

        public string Title { get; set; }

        public int Level { get; set; }

        public bool Published { get; set; }

        public string StandardPdf { get; set; }

        public string AssessmentPlanPdf { get; set; }

        public List<string> JobRoles { get; set; }

        public List<string> Keywords { get; set; }

        public int Duration { get; set; }

        public int FundingCap { get; set; }

        public string IntroductoryText { get; set; }

        public string EntryRequirements { get; set; }

        public string WhatApprenticesWillLearn { get; set; }

        public string Qualifications { get; set; }

        public string ProfessionalRegistration { get; set; }

        public string OverviewOfRole { get; set; }
    }
}
