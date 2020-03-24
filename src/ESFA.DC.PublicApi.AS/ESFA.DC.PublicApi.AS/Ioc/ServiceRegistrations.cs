using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.Api.Common.Settings;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF.Valid;
using ESFA.DC.ILR1920.DataStore.EF.Valid.Interface;
using ESFA.DC.PublicApi.AS.Services;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PublicApi.AS.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ILR1920_DataStoreEntitiesValid>().As<IIlr1920ValidContext>().InstancePerLifetimeScope();
            builder.RegisterType<ILR1920_DataStoreEntities>().As<IIlr1920RulebaseContext>().InstancePerLifetimeScope();
            builder.RegisterType<LearnersRepository>().As<ILearnersRepository>();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1920_DataStoreEntitiesValid>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.ConnectionStringsDictionary["ILR1920DataStore"],
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
                        connectionStrings.ConnectionStringsDictionary["ILR1920DataStore"],
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1920_DataStoreEntities>>()
                .SingleInstance();

        }
    }
}