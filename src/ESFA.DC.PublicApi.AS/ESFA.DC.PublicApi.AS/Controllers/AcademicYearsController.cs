using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Api.Common.Extensions;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PublicApi.AS.Constants;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.PublicApi.AS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = PolicyNameConstants.DataAccess)]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/academic-years")]
    [Produces("application/json")]
    [ApiController]
    public class AcademicYearsController : Controller
    {
        private readonly IAcademicYearsRepository _academicYearsRepository;
        private readonly ILogger _logger;

        public AcademicYearsController(IAcademicYearsRepository academicYearsRepository, ILogger logger)
        {
            _academicYearsRepository = academicYearsRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets list of available academic years in descending order e.g. 1920,1819
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="dateTimeUtc"></param>
        /// /// <param name="includeClosed"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>List of academic years list    with response header named "X-pagination" for paging information containing following
        ///  int TotalItems
        ///  int PageNumber
        ///  int PageSize
        ///  int TotalPages.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAcademicYears(CancellationToken cancellationToken, [FromQuery] DateTime? dateTimeUtc = null, [FromQuery] bool? includeClosed = null, [FromQuery] int? pageSize = null, [FromQuery] int? pageNumber = null)
        {
            try
            {
                var result = await _academicYearsRepository.GetAcademicYears(
                                cancellationToken, 
                                dateTimeUtc,
                                includeClosed,
                                pageSize ?? DefaultConstants.DefaultPageSize,
                                pageNumber ?? DefaultConstants.DefaultPageNumber);

                if (result?.TotalItems > 0)
                {
                    Response.AddPaginationHeader(result);

                    _logger.LogDebug($"Call to GetAcademicYears completed with data count : {result.TotalItems}");
                    return Ok(result.List);
                }

                _logger.LogDebug("Call to GetAcademicYears completed with no data");
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting GetAcademicYears", e);
                return BadRequest();
            }
        }
    }
}
