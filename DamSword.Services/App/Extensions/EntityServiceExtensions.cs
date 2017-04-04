using System;
using System.Linq.Expressions;
using DamSword.Data;
using DamSword.Services.Exceptions;

namespace DamSword.Services
{
    public static class EntityServiceExtensions
    {
        public static EntityValidationException<TEntity> ValidationException<TEntity>(this IEntityService<TEntity> self, Expression<Func<TEntity, object>> fieldSelector, string message)
            where TEntity : IEntity
        {
            return new EntityValidationException<TEntity>(fieldSelector, message);
        }
    }
}