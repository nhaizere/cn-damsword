using DamSword.Common;
using DamSword.Common.Events;

namespace DamSword.Data.Events
{
    public interface IEntityAddedEventSubscriber<TEntity> : IEventSubscriber<EntityAddedEvent<TEntity>>
        where TEntity : IEntity
    {
    }
}
