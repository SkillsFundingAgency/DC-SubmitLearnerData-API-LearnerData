using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PublicApi.AS.Services
{
    public class LearnerApiAvailabilityService : ILearnerApiAvailabilityService
    {
        private readonly Func<IJobQueueDataContext> _jobQueDatacontextFactory;

        private const string LearnerApi = "Learner";
        public LearnerApiAvailabilityService(Func<IJobQueueDataContext> jobQueDatacontextFactory)
        {
            _jobQueDatacontextFactory = jobQueDatacontextFactory;
        }

        public async Task<bool> IsLearnerApiAvailableAsync(CancellationToken cancellationToken)
        {
            using (var context = _jobQueDatacontextFactory())
            {
                var isApiDisabled = await context.ApiAvailability.AnyAsync(x => x.ApiName == LearnerApi && x.Enabled == false);

                return !isApiDisabled;
            }
        }
    }
}
