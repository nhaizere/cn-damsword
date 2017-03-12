using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DamSword.Common;

namespace DamSword.Data.Events
{
    public class EntityModifiedEvent<TEntity> : EntityEventBase<TEntity>
        where TEntity : IEntity
    {
        public IEnumerable<EntityModifiedProperty> ModifiedProperties { get; set; }

        public bool IsModified<TValue>(Expression<Func<TEntity, TValue>> propertySelector)
        {
            var propertyName = propertySelector.GetPropertyName();
            return ModifiedProperties.Any(p => p.PropertyName == propertyName);
        }

        public TValue GetOriginalValue<TValue>(Expression<Func<TEntity, TValue>> propertySelector)
        {
            var propertyName = propertySelector.GetPropertyName();
            if (ModifiedProperties.All(p => p.PropertyName != propertyName))
                propertySelector.Compile().Invoke(Entity);

            return ModifiedProperties.Single(p => p.PropertyName == propertyName).GetOriginalValue<TValue>();
        }
    }
}
