using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Api.Common.Paging.Interfaces;
using ESFA.DC.Api.Common.Paging.Pagination;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PublicApi.AS.Services
{
    public class AcademicYearsRepository : IAcademicYearsRepository
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly Func<IJobQueueDataContext> _jobQueDatacontextFactory;
        private readonly string IlrCollectionType = "ILR";

        public AcademicYearsRepository(IDateTimeProvider dateTimeProvider,Func<IJobQueueDataContext> jobQueDatacontextFactory)
        {
            _dateTimeProvider = dateTimeProvider;
            _jobQueDatacontextFactory = jobQueDatacontextFactory;
        }

        public async Task<IPaginatedResult<int>> GetAcademicYears(CancellationToken cancellationToken, DateTime? dateTimeUtc = null, bool? includeClosed = false, int pageSize = 0, int pageNumber = 0)
        {
            dateTimeUtc ??= _dateTimeProvider.GetNowUtc();

            using (var context = _jobQueDatacontextFactory())
            {
                var collections = await context.ReturnPeriod
                    .Where(x => x.StartDateTimeUtc <= dateTimeUtc && x.Collection.CollectionType.Type == IlrCollectionType)
                    .Select(x => x.Collection.CollectionId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var data = context.ReturnPeriod
                    .Where(x => collections.Contains(x.CollectionId))
                    .GroupBy(x => new {x.Collection.CollectionYear})
                    .Select(x => new
                    {
                        CollectionYear = x.Key.CollectionYear.Value,
                        MaxPeriodClosedDate = x.Max(y => y.EndDateTimeUtc)
                    })
                    .Where(y => includeClosed.GetValueOrDefault() || y.MaxPeriodClosedDate >= dateTimeUtc)
                    .Select(x => x.CollectionYear).OrderByDescending(x => x);

                var result = await PaginatedResultFactory<int>.CreatePaginatedResultAsync(data,pageSize, pageNumber,cancellationToken);
                return result;
            }
        }
    }
}
