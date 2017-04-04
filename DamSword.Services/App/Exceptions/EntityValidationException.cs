using System;
using System.Linq.Expressions;
using DamSword.Data;
using DamSword.Common;

namespace DamSword.Services.Exceptions
{
    public class EntityValidationException<TEntity> : Exception
        where TEntity : IEntity
    {
        public Expression<Func<TEntity, object>> FieldSelector { get; }
        public string FieldName => FieldSelector.AsMemberExpressionSkipConvert().Member.Name;

        public EntityValidationException(Expression<Func<TEntity, object>> fieldSelector, string message)
            : base(message ?? throw new ArgumentNullException(nameof(message)))
        {
            if (fieldSelector.GetPropertyName(dontThrowIfNotResolved: true) == null)
                throw new ArgumentException("Must contain single accessed property.", nameof(fieldSelector));

            FieldSelector = fieldSelector ?? throw new ArgumentNullException(nameof(fieldSelector));
        }
    }
}