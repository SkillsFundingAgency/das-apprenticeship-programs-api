using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfa.Das.ApprenticeshipInfoService.Core.Models;

namespace Sfa.Das.ApprenticeshipInfoService.Core.Services
{
    public interface IPostCodeIoLocator
    {
        Task<PostCodeResponse> GetLatLongFromPostcode(string postCode);
    }
}
