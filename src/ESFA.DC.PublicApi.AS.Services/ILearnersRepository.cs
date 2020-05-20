using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Api.Common.Paging.Interfaces;

namespace ESFA.DC.PublicApi.AS.Services
{
    public interface ILearnersRepository
    {

        Task<IPaginatedResult<int>> GetSubmittingProviders(CancellationToken cancellationToken, DateTime? startDateTime = null, int pageSize = 0, int pageNumber = 0);

        Task<IPaginatedResult<Dtos.Learner>> GetLearners(
            CancellationToken cancellationToken,
            DateTime? startDateTime = null,
            int? ukprn = null,
            int? aimType = 1,
            int? standardCode = null,
            int[] fundModel = null,
            int? progType = null,
            int pageSize = 0,
            int pageNumber = 0);
    }
}
