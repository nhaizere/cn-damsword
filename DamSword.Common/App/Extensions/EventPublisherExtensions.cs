using System;
using System.Collections.Generic;
using DamSword.Common.Events;

namespace DamSword.Common
{
    public static class EventPublisherExtensions
    {
        public static void PublishEvents(this IEventPublisher self, IEnumerable<object> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
            {
                self.PublishEvent(@event);
            }
        }
    }
}