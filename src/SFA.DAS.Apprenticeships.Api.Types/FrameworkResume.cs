using System;
using System.Collections.Generic;

namespace SFA.DAS.Apprenticeships.Api.Types
{
    public sealed class FrameworkResume
    {
        /// <summary>
        /// a link to the framework information
        /// </summary>
        public string Uri { get; set; }

        public string Title { get; set; }

        public int FrameworkCode { get; set; }

        public double Ssa1 { get; set; }

        public double Ssa2 { get; set; }
    }
}
