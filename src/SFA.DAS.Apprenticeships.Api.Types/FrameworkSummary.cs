﻿using System;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public sealed class FrameworkSummary
    {
        /// <summary>
        /// A composite framework Id {framework-code}-{program-type}-{pathway-code}
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// a link to the framework details
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// a unique title for framework and pathway
        /// </summary>
        public string Title { get; set; }

        public string FrameworkName { get; set; }

        public string PathwayName { get; set; }

        public int ProgType { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }

        public int Level { get; set; }

        [Obsolete("Use 'Duration' Instead.")]
        public TypicalLength TypicalLength { get; set; }

        public int Duration { get; set; }

        public int MaxFunding { get; set; }
    }
}
