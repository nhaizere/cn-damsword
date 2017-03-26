using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using DamSword.Common.Events;

namespace DamSword.Web
{
    public class EventPublisher : IEventPublisher
    {
        public ILifetimeScope LifetimeScope { get; set; }

        public void PublishEvent(object @event)
        {
            var eventSubscriberType = GetEventSubscriberType(@event);
            var eventSubscribersType = GetEventSubscribersType(eventSubscriberType);
            var subscribers = (IEnumerable<object>)LifetimeScope.Resolve(eventSubscribersType);
            var orderedSubscribers = subscribers
                .OrderBy(s => (s as IOrderedEventSubscriber)?.HandleEventOrder)
                .ToArray();

            foreach (var subscriber in orderedSubscribers)
            {
                var handleEventMethodInfo = eventSubscriberType.GetMethod("HandleEvent");
                handleEventMethodInfo.Invoke(subscriber, new[] { @event });
            }
        }

        private static Type GetEventSubscriberType(object @event)
        {
            var eventType = @event.GetType();
            return typeof(IEventSubscriber<>).MakeGenericType(eventType);
        }

        private static Type GetEventSubscribersType(Type eventSubscriberType)
        {
            return typeof(IEnumerable<>).MakeGenericType(eventSubscriberType);
        }
    }
}