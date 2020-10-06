using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.PublicApi.AS.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly HttpClient _httpClient;

        public HttpClientService(IJsonSerializationService jsonSerializationService, HttpClient  httpClient)
        {
            _jsonSerializationService = jsonSerializationService;
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            var data = await GetDataAsync(url, cancellationToken);

            return string.IsNullOrWhiteSpace(data) ? default(T) : _jsonSerializationService.Deserialize<T>(data);
        }

        public async Task<string> GetDataAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(new Uri(url), cancellationToken);
            response.EnsureSuccessStatusCode();

            return response.StatusCode == HttpStatusCode.NoContent ? null : await response.Content.ReadAsStringAsync();
        }
    }
}
