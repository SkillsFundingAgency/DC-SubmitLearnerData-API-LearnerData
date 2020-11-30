using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using ESFA.DC.PublicApi.AS.Settings;

namespace ESFA.DC.PublicApi.AS.Services
{
    public class LearnerApiAvailabilityService : ILearnerApiAvailabilityService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        private const string LearnerApi = "Learner";
        public LearnerApiAvailabilityService(
            LearnerApiSettings learnerApiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = learnerApiSettings.JobManagementApiBaseUrl;
        }

        public async Task<bool> IsLearnerApiAvailableAsync(CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/api/apiavailability/{LearnerApi}";
            return await _httpClientService.GetAsync<bool>(url, cancellationToken);
        }
    }
}
