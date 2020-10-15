using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESFA.DC.PublicApi.AS.Filters
{
    public  class ApiAvailabilityFilter : IAsyncActionFilter
    {
        private readonly ILearnerApiAvailabilityService _learnerApiAvailabilityService;
        private readonly ILogger _logger;

        public ApiAvailabilityFilter(ILearnerApiAvailabilityService learnerApiAvailabilityService, ILogger logger)
        {
            _learnerApiAvailabilityService = learnerApiAvailabilityService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cancellationToken = context.HttpContext.RequestAborted;
            var apiAvailable = await _learnerApiAvailabilityService.IsLearnerApiAvailableAsync(cancellationToken);
            if (!apiAvailable)
            {
                _logger.LogDebug($"Learner Api is not available.");
                context.Result = new NoContentResult();
                return;
            }

            await next();
        }
    }
}
