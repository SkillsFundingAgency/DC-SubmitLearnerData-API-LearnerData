using Autofac;
using ESFA.DC.Api.Common.Settings.Extensions;
using ESFA.DC.PublicApi.AS.Settings;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.PublicApi.AS.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupLearnerApiConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c =>
                    configuration.GetConfigSection<LearnerApiSettings>())
                .As<LearnerApiSettings>().SingleInstance();
        }
    }
}