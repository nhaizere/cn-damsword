using System;
using System.Collections.Generic;
using System.Linq;
using DamSword.Data.Events;
using DamSword.Data.Repositories;

namespace DamSword.Data
{
    public static class EntityRepositoryExtensions
    {
        public static IEnumerable<EntityEventBase<TEntity>> GetEntityEvents<TEntity>(this EntityRepositoryBase<TEntity> self)
            where TEntity : class, IEntity
        {
            var changes = self.GetChanges();
            var events = changes.Select(change =>
            {
                EntityEventBase<TEntity> @event;
                switch (change.State)
                {
                    case EntityChangeState.Added:
                        @event = new EntityAddedEvent<TEntity>();
                        break;
                    case EntityChangeState.Deleted:
                        @event = new EntityDeletedEvent<TEntity>();
                        break;
                    case EntityChangeState.Modified:
                        @event = new EntityModifiedEvent<TEntity> { ModifiedProperties = change.ModifiedProperties };
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown Entity change state: \"{change.State}\".");
                }

                @event.Entity = change.Entity;
                return @event;
            }).ToArray();

            return events;
        }
    }
}
