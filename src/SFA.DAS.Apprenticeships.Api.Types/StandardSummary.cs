using System;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public class StandardSummary
    {
        /// <summary>
        /// The standard identifier from LARS
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// a link to the standard details
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The standard title
        /// </summary>
        public string Title { get; set; }

        [Obsolete("Use 'Duration' Instead.")]
        public TypicalLength TypicalLength { get; set; }

        public int Duration { get; set; }

        public int MaxFunding { get; set; }

        public int Level { get; set; }

        public bool IsPublished { get; set; }

        public double Ssa1 { get; set; }

        public double Ssa2 { get; set; }
    }
}
