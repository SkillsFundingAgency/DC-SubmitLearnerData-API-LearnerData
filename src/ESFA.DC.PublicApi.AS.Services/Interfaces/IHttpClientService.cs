using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.PublicApi.AS.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string url, CancellationToken cancellationToken);
    }
}
