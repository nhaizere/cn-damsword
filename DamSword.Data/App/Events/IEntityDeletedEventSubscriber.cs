using DamSword.Common.Events;

namespace DamSword.Data.Events
{
    public interface IEntityDeletedEventSubscriber<TEntity> : IEventSubscriber<EntityDeletedEvent<TEntity>>
        where TEntity : IEntity
    {
    }
}
