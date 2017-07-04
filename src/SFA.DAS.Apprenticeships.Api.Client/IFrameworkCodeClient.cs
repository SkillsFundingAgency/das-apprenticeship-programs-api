using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public interface IFrameworkCodeClient
    {
        bool Exists(int frameworkCode);

        Task<bool> ExistsAsync(int frameworkCode);

        FrameworkCodeSummary Get(int frameworkCode);

        IEnumerable<FrameworkCodeSummary> FindAll();

        Task<IEnumerable<FrameworkCodeSummary>> FindAllAsync();

        Task<FrameworkCodeSummary> GetAsync(int frameworkCode);
    }
}