using Newtonsoft.Json;

namespace ESFA.DC.PublicApi.AS.Settings
{
    public class ConnectionStrings : Api.Common.Settings.ConnectionStrings
    {
        [JsonRequired]
        public string ILR1920DataStore { get; set; }

        public override string AppLogs { get; set; }

        public override string WebApiExternalIdentity { get; set; }
    }
}
