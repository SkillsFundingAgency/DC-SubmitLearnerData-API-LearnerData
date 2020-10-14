using System.Threading;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESFA.DC.PublicApi.AS.Filters
{
    public  class ApiAvailabilityFilter : IActionFilter
    {
        private readonly ILearnerApiAvailabilityService _learnerApiAvailabilityService;
        private readonly ILogger _logger;

        public ApiAvailabilityFilter(ILearnerApiAvailabilityService learnerApiAvailabilityService, ILogger logger)
        {
            _learnerApiAvailabilityService = learnerApiAvailabilityService;
            _logger = logger;
        }


        public void OnActionExecuting(ActionExecutingContext context)
        {
            var apiAvailable = _learnerApiAvailabilityService.IsLearnerApiAvailableAsync(CancellationToken.None).Result;
            if (!apiAvailable)
            {
                _logger.LogDebug($"Learner Api is not available.");
                context.Result = new NoContentResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //do nothing
        }
    }
}
