using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;

namespace SFA.DAS.Apprenticeships.Api.Client
{
    public interface IFrameworkCodeClient
    {
        bool Exists(int frameworkCode);

        bool Exists(string frameworkCode);

        Task<bool> ExistsAsync(int frameworkCode);

        Task<bool> ExistsAsync(string frameworkCode);

        FrameworkCodeSummary Get(int frameworkCode);

        FrameworkCodeSummary Get(string frameworkCode);

        IEnumerable<FrameworkCodeSummary> FindAll();

        Task<IEnumerable<FrameworkCodeSummary>> FindAllAsync();

        Task<FrameworkCodeSummary> GetAsync(int frameworkCode);

        Task<FrameworkCodeSummary> GetAsync(string frameworkCode);
    }
}