using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Api.Common.Paging.Interfaces;

namespace ESFA.DC.PublicApi.AS.Services.Interfaces
{
    public interface IAcademicYearsRepository
    {
        Task<IPaginatedResult<int>> GetAcademicYears(CancellationToken cancellationToken, DateTime? dateTimeUtc = null, bool? includeClosed = false, int pageSize = 0, int pageNumber = 0);
    }
}
