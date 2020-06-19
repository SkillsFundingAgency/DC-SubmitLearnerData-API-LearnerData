using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.Api.Common.Extensions;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PublicApi.AS.Constants;
using ESFA.DC.PublicApi.AS.Services;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.PublicApi.AS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = PolicyNameConstants.DataAccess)]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ilr-data")]
    [ApiController]
    public class IlrDataController : Controller
    {
        private readonly ILogger _logger;
        private readonly IIndex<int, IlrRepository> _ilrRepositories;

        public IlrDataController(ILogger logger, IIndex<int, IlrRepository> ilrRepositories)
        {
            _logger = logger;
            _ilrRepositories = ilrRepositories;
        }

        ///  <summary>
        ///  Gets providers based on criteria
        ///  </summary>
        /// <param name="cancellationToken"></param>
        ///  <param name="academicYear"></param>
        ///  <param name="startDateTime">filter based on when provider file submission date time - this date time will be used to get records from that date time onwards</param>
        ///  <param name="pageSize"></param>
        ///  <param name="pageNumber"></param>
        ///  <returns>List of providers dto with response header named "X-pagination" for paging information containing following
        ///   int TotalItems
        ///   int PageNumber
        ///   int PageSize
        ///   int TotalPages.
        ///  </returns>
        [HttpGet("providers/{academicYear}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<int>>> GetProviders(CancellationToken cancellationToken,int academicYear, [FromQuery]DateTime? startDateTime = null, [FromQuery]int? pageSize = null, [FromQuery]int? pageNumber = null)
        {
            try
            {
                _ilrRepositories.TryGetValue(academicYear, out var repository);

                if (repository == null)
                {
                    _logger.LogDebug($"Call to GetProviders completed with no content - academic year {academicYear} not supported.");
                    return NoContent();
                }

                var providerResults = await repository.GetSubmittingProviders(cancellationToken, startDateTime, pageSize ?? DefaultConstants.DefaultPageSize, pageNumber ?? DefaultConstants.DefaultPageNumber);

                if (providerResults?.TotalItems > 0)
                {
                    Response.AddPaginationHeader(providerResults);

                    _logger.LogDebug($"Call to GetProviders completed with data count : {providerResults.TotalItems}");
                    return Ok(providerResults.List);
                }

                _logger.LogDebug("Call to GetProviders completed with no data");
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting providers for startdatetime :{startDateTime}", e);
                return BadRequest();
            }
        }

        ///  <summary>
        ///  Get learners and learning deliveries based on filter criteria
        ///  </summary>
        ///  <remarks>
        ///  Records can be filtered by passing either no filter criteria, ukprn or start datetime (passing ukprn and start date time together will result in 400 error)
        ///  </remarks>
        /// <param name="cancellationToken"></param>
        ///  <param name="academicYear"></param>
        ///  <param name="startDateTime">filter based on when provider file submission date time - this date time will be used to get records from that date time onwards</param>
        ///  <param name="ukprn">pass this value in if learners should be filtered by a ukprn</param>
        ///  <param name="aimType"></param>
        ///  <param name="standardCode">passing Null will return learners with ANY learning delivery => standard code null,
        ///  pass -1 if all learners should be returned regardless of standard code value for learning delivery</param>
        ///  <param name="fundModel">Ignore the parameter to include all fund models or add fund model values as parameter(s). Learners will be filtered based on having at least one of the learning delivery with matching fund model(s)</param>
        ///  <param name="progType">passing Null will return learners with ANY learning delivery => prog type null,
        ///  pass -1 if all learners should be returned regardless of prog type value for learning delivery</param>
        ///  <param name="pageNumber"></param>
        ///  <param name="pageSize"></param>
        ///  <returns>List of learners dto with response header named "X-pagination" for paging information containing following
        ///   int TotalItems
        ///   int PageNumber
        ///   int PageSize
        ///   int TotalPages.
        ///  </returns>
        [HttpGet("learners/{academicYear}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Dtos.Learner>> GetLearners(
            CancellationToken cancellationToken,
            int academicYear,
            [FromQuery]DateTime? startDateTime = null,
           [FromQuery]int? ukprn = null,
           [FromQuery]int? aimType = null,
           [FromQuery]int? standardCode = null,
           [FromQuery]int[] fundModel = null,
           [FromQuery]int? progType = null,
           [FromQuery]int? pageSize = null,
           [FromQuery]int? pageNumber = null)
        {
            try
            {
                if (startDateTime.HasValue && ukprn.HasValue)
                {
                    _logger.LogDebug("start date time and ukprn both were passed in, you can only filter by one or the other");
                    return BadRequest("start date time and ukprn both were passed in, you can only filter by one or the other");
                }

                _ilrRepositories.TryGetValue(academicYear, out var repository);

                if (repository == null)
                {
                    _logger.LogDebug($"Call to GetLearners completed with no content - academic year {academicYear} not supported.");
                    return NoContent();
                }

                var learnersResult = await repository.GetLearners(
                                    cancellationToken,
                                    startDateTime,
                                    ukprn,
                                    aimType,
                                    standardCode,
                                    fundModel,
                                    progType,
                                    pageSize ?? DefaultConstants.DefaultPageSize,
                                    pageNumber ?? DefaultConstants.DefaultPageNumber);

                if (learnersResult?.TotalItems > 0)
                {
                    Response.AddPaginationHeader(learnersResult);

                    _logger.LogDebug($"Call to GetLearners completed with data, count : {learnersResult.TotalItems}");
                    return Ok(learnersResult.List);
                }

                _logger.LogDebug("Call to GetLearners completed with no data");
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting learners for start datetime :{startDateTime}", e);
                return BadRequest();
            }
        }
    }
}