using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.PublicApi.AS.Services.Interfaces
{
    public interface ILearnerApiAvailabilityService
    {
        Task<bool> IsLearnerApiAvailableAsync(CancellationToken cancellationToken);
    }
}
