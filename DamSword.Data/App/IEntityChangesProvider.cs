using System.Collections.Generic;

namespace DamSword.Data
{
    public enum EntityChangeState
    {
        Added,
        Deleted,
        Modified
    }

    public struct EntityModifiedProperty
    {
        public string PropertyName { get; set; }
        public object OriginalValue { get; set; }
        public object CurrentValue { get; set; }

        public TValue GetCurrentValue<TValue>()
        {
            return (TValue)CurrentValue;
        }

        public TValue GetOriginalValue<TValue>()
        {
            return (TValue)OriginalValue;
        }
    }

    public class EntityChange<TEntity>
        where TEntity : class, IEntity
    {
        public TEntity Entity { get; set; }
        public EntityChangeState State { get; set; }
        public IEnumerable<EntityModifiedProperty> ModifiedProperties { get; set; }
    }

    public interface IEntityChangesProvider
    {
        IEnumerable<EntityChange<TEntity>> GetEntityChanges<TEntity>()
            where TEntity : class, IEntity;
    }
}
