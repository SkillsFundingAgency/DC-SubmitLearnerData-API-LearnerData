using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Api.Common.Paging.Interfaces;
using ESFA.DC.Api.Common.Paging.Pagination;
using ESFA.DC.ILR2021.DataStore.EF;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PublicApi.AS.Services
{
    public class Ilr2021Repository : IlrRepository
    {
        private readonly Func<IIlr2021Context> _ilr2021Context;

        public Ilr2021Repository(Func<IIlr2021Context> ilr2021Context)
        {
            _ilr2021Context = ilr2021Context;
        }

        public async Task<IPaginatedResult<int>> GetSubmittingProviders(CancellationToken cancellationToken, DateTime? startDateTime = null, int pageSize = 0, int pageNumber = 0)
        {
            IPaginatedResult<int> providers;
            using (var context = _ilr2021Context())
            {
                var query = GetSubmittedProviders(context, startDateTime);
                providers = await PaginatedResultFactory<int>.CreatePaginatedResultAsync(query, pageSize, pageNumber, cancellationToken);
            }

            return providers;
        }

        public async Task<IPaginatedResult<Dtos.Learner>> GetLearners(
            CancellationToken cancellationToken,
            DateTime? startDateTime = null,
            int? ukprn = null,
            int? aimType = null,
            int? standardCode = null,
            int[] fundModel = null,
            int? progType = null,
            int pageSize = 0,
            int pageNumber = 0)
        {
            List<int> providers = null ;
            IPaginatedResult<Dtos.Learner> learners;


            if (ukprn.HasValue)
            {
                providers = new List<int>() { ukprn.GetValueOrDefault() };
            }
            else
            {
                using (var context = _ilr2021Context())
                {
                    var providersQuery = GetSubmittedProviders(context, startDateTime);
                    providers = await providersQuery.ToListAsync(cancellationToken);
                }
            }

            using (var context = _ilr2021Context())
            {
                var query = context.Learners
                    .Where(x => (providers.Contains(x.UKPRN))
                                && x.LearningDeliveries != null
                                && (fundModel == null || fundModel.Length == 0 || x.LearningDeliveries.Any(y => fundModel.Contains(y.FundModel)))
                                && x.LearningDeliveries.Any(y => !aimType.HasValue || y.AimType == aimType.Value)
                                && x.LearningDeliveries.Any(z => (standardCode.HasValue && standardCode.Value == -1) || z.StdCode == standardCode)
                                && x.LearningDeliveries.Any(z => (progType.HasValue && progType.Value == -1) || z.ProgType == progType))
                    .AsQueryable();

                var data = ProjectLearners(query);
                learners = await PaginatedResultFactory<Dtos.Learner>.CreatePaginatedResultAsync(data, pageSize, pageNumber, cancellationToken);
            }

            return learners;

        }

        private IQueryable<Dtos.Learner> ProjectLearners(IQueryable<Learner> learners)
        {
            var data = learners.Select(l =>
                new Dtos.Learner
                {
                    DateOfBirth = l.DateOfBirth,
                    FamilyName = l.FamilyName,
                    GivenNames = l.GivenNames,
                    LearnRefNumber = l.LearnRefNumber,
                    NINumber = l.NINumber,
                    Ukprn = l.UKPRN,
                    Uln = l.ULN,
                    LearningDeliveries = l.LearningDeliveries.Select(ld => new Dtos.LearningDelivery
                    {
                        AchDate = ld.AchDate,
                        AimType = ld.AimType,
                        CompStatus = ld.CompStatus,
                        DelLocPostCode = ld.DelLocPostCode,
                        EPAOrgID = ld.EPAOrgID,
                        FundModel = ld.FundModel,
                        LearnActEndDate = ld.LearnActEndDate,
                        LearnPlanEndDate = ld.LearnPlanEndDate,
                        LearnStartDate = ld.LearnStartDate,
                        OutGrade = ld.OutGrade,
                        Outcome = ld.Outcome,
                        StdCode = ld.StdCode,
                        WithdrawReason = ld.WithdrawReason,
                        ProgType = ld.ProgType
                    }).ToList()
                });

            return data;
        }

        private IQueryable<int> GetSubmittedProviders(IIlr2021Context context, DateTime? startDateTime = null)
        {
            var query = context.FileDetails
                .Where(x => x.SubmittedTime > (startDateTime ?? new DateTime(1999, 1, 1)))
                .Select(x => x.UKPRN)
                .OrderBy(x => x)
                .Distinct()
                .AsQueryable();

            return query;
        }
    }
}
