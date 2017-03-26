using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Castle.DynamicProxy;
using DamSword.Common;
using DamSword.Common.Events;
using DamSword.Data;
using DamSword.Data.Events;

namespace DamSword.Web.Events
{
    public class EntityChangeEventPublisherEntityContextInterceptor : IInterceptor
    {
        public ILifetimeScope LifetimeScope { get; set; }
        public IEventPublisher EventPublisher { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var entityContext = invocation.InvocationTarget as IEntityContext;
            if (entityContext == null)
                throw new InvalidOperationException($"Invocation Target must be of type {typeof(IEntityContext).Name}.");

            if (invocation.Method.Name != "SaveChanges")
            {
                invocation.Proceed();
                return;
            }

            var events = ResolveEntityEvents();
            invocation.Proceed();
            EventPublisher.PublishEvents(events);
            invocation.Proceed(); // extra SaveChanges after publishing events
        }

        private IEnumerable<object> ResolveEntityEvents()
        {
            var providers = GetEntityEventsProvidersGenerator().ToArray();
            var eventList = new List<object>();
            foreach (var provider in providers)
            {
                var methods = provider.GetType()
                    .GetInterfaces()
                    .Where(t => t.ImplementsGenericType(typeof(IEntityEventsProvider<>)))
                    .SelectMany(t => t.GetMethods())
                    .Where(m => m.Name == ExpressionExtensions.GetMethodName<Action<IEntityEventsProvider<object>>>(p => p.GetEntityEvents()))
                    .ToArray();

                foreach (var method in methods)
                {
                    var events = (IEnumerable<object>)method.Invoke(provider, new object[0]);
                    eventList.AddRange(events);
                }
            }

            return eventList;
        }

        private IEnumerable<object> GetEntityEventsProvidersGenerator()
        {
            var providerTypes = LifetimeScope.GetGenericImplementingTypes(typeof(IEntityEventsProvider<>));
            var registeredProviderTypes = providerTypes.Select(LifetimeScope.GetRegisteredType).Distinct().ToArray();
            foreach (var providerType in registeredProviderTypes)
            {
                yield return LifetimeScope.Resolve(providerType);
            }
        }
    }
}