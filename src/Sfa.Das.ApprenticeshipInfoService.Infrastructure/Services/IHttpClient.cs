using System;
using System.Threading.Tasks;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services
{
    public interface IHttpClient
    {
        Task<string> GetAsync(Uri uri);
    }
}