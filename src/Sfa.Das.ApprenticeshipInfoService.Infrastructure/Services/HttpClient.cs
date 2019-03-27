using System;
using System.Threading.Tasks;

namespace Sfa.Das.ApprenticeshipInfoService.Infrastructure.Services
{
    public class HttpClient : IHttpClient
    {
        public async Task<string> GetAsync(Uri uri)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
