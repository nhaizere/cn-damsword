using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.Variance;
using DamSword.Common;
using DamSword.Common.Events;
using DamSword.Data;
using DamSword.Data.Repositories;
using DamSword.Providers;
using DamSword.Services;
using DamSword.Web.Events;

namespace DamSword.Web
{
    public class DependenciesConfig
    {
        private static readonly WeakReference<IEnumerable<Type>> AssemblyTypesReference = new WeakReference<IEnumerable<Type>>(null);
        private static readonly WeakReference<Parameter> AppConfigParameterReference = new WeakReference<Parameter>(null);

        public static IEnumerable<Type> AssemblyTypes
        {
            get
            {
                return AssemblyTypesReference.GetOrFetchTarget(() =>
                {
                    var assemblies = new[]
                    {
                        Assembly.Load(new AssemblyName("DamSword.Common")),
                        Assembly.Load(new AssemblyName("DamSword.Data")),
                        Assembly.Load(new AssemblyName("DamSword.Providers")),
                        Assembly.Load(new AssemblyName("DamSword.Services")),
                        Assembly.Load(new AssemblyName("DamSword.Web"))
                    };

                    return assemblies
                        .SelectMany(a => a.GetTypes().Where(t => !t.GetTypeInfo().IsAbstract && t.Namespace != null && t.Namespace.StartsWith("DamSword.")))
                        .ToArray();
                });
            }
        }

        public static Parameter AppConfigParameter
        {
            get
            {
                return AppConfigParameterReference.GetOrFetchTarget(() =>
                {
                    var appConfigParameterDictionary = new Dictionary<string, object>
                    {
                    };

                    return new ResolvedParameter(
                        (p, ctx) => appConfigParameterDictionary.ContainsKey(p.Name) && p.ParameterType == appConfigParameterDictionary[p.Name].GetType(),
                        (p, ctx) => appConfigParameterDictionary[p.Name]);
                });
            }
        }

        public static void Configure(ContainerBuilder builder, bool isProduction)
        {
            builder.RegisterSource(new ContravariantRegistrationSource());
            
            var repositoryTypes = AssemblyTypes.Where(t => t.ImplementsGenericType(typeof(IEntityRepository<>)) | t.ImplementsInterface<IProvider>() | t.ImplementsInterface<IService>()).ToArray();
            foreach (var repositoryType in repositoryTypes)
            {
                builder.RegisterType(repositoryType)
                    .AsImplementedInterfaces()
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                    .WithParameter(new ResolvedParameter((p, ctx) => p.Name == "asNoTracking", (p, ctx) => false))
                    .InstancePerLifetimeScope();
            }

            var eventHandlerTypes = AssemblyTypes.Where(t => t.Name.EndsWith("EventHandler")).ToArray();
            foreach (var eventHandlerType in eventHandlerTypes)
            {
                builder.RegisterType(eventHandlerType)
                    .AsImplementedInterfaces()
                    .PropertiesAutowired()
                    .WithParameter(AppConfigParameter)
                    .InstancePerLifetimeScope();
            }

            builder.RegisterType<EntityChangeEventPublisherEntityContextInterceptor>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).InstancePerLifetimeScope();
            builder.RegisterType<EntityContext>()
                .As<IEntityContext>()
                .PropertiesAutowired()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(EntityChangeEventPublisherEntityContextInterceptor))
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().PropertiesAutowired().InstancePerLifetimeScope();
        }
    }
}