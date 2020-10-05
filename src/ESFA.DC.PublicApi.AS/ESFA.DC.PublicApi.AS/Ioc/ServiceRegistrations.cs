using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.Api.Common.Settings;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Valid;
using ESFA.DC.ILR1920.DataStore.EF.Valid.Interface;
using ESFA.DC.ILR2021.DataStore.EF;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.JobQueueManager;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.PublicApi.AS.Services;
using ESFA.DC.PublicApi.AS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PublicApi.AS.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ILR1920_DataStoreEntitiesValid>().As<IIlr1920ValidContext>().ExternallyOwned();
            builder.RegisterType<ILR1920_DataStoreEntities>().As<IIlr1920RulebaseContext>().ExternallyOwned();
            builder.RegisterType<ILR2021_DataStoreEntities>().As<IIlr2021Context>().ExternallyOwned();

            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();

            builder.RegisterType<Ilr1920Repository>().Keyed<IlrRepository>(1920).WithAttributeFiltering().ExternallyOwned();
            builder.RegisterType<Ilr2021Repository>().Keyed<IlrRepository>(2021).WithAttributeFiltering().ExternallyOwned();

            builder.RegisterType<AcademicYearsRepository>().As<IAcademicYearsRepository>();
            builder.RegisterType<LearnerApiAvailabilityService>().As<ILearnerApiAvailabilityService>();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["JobManagement"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1920_DataStoreEntitiesValid>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["ILR1920DataStore"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1920_DataStoreEntitiesValid>>()
                .SingleInstance();


            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1920_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["ILR1920DataStore"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1920_DataStoreEntities>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR2021_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.KeyValues["ILR2021DataStore"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR2021_DataStoreEntities>>()
                .SingleInstance();

        }
    }
}