using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.PublicApi.AS.Filters
{
    public sealed class ApiAvailabilityAttribute : TypeFilterAttribute
    {
        public ApiAvailabilityAttribute() : base(typeof(ApiAvailabilityFilter))
        {
        }
    }
}
