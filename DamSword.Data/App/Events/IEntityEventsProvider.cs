using System.Collections.Generic;

namespace DamSword.Data.Events
{
    public interface IEntityEventsProvider<TEntity>
    {
        IEnumerable<IEntityEvent<TEntity>> GetEntityEvents();
    }
}
