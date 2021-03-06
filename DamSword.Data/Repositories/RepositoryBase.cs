﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DamSword.Common;
using DamSword.Data.Events;

namespace DamSword.Data.Repositories
{
    public interface IEntityRepository<TEntity> : IEntityEventsProvider<TEntity>, IService
        where TEntity : IEntity
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);

        TEntity GetById(long id);
        TResult GetById<TResult>(long id, Expression<Func<TEntity, TResult>> selector);
        TEntity TryGetById(long id);
        TResult TryGetById<TResult>(long id, Expression<Func<TEntity, TResult>> selector);
        IQueryable<TEntity> Select(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TResult> Select<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        IQueryable<TEntity> SelectAll();
        IQueryable<TResult> SelectAll<TResult>(Expression<Func<TEntity, TResult>> selector);
        IQueryable<TResult> SelectMany<TResult>(Expression<Func<TEntity, IEnumerable<TResult>>> selector);
        IQueryable<TResult> SelectMany<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, IEnumerable<TResult>>> selector);
        TEntity First(Expression<Func<TEntity, bool>> predicate);
        TResult First<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        TResult FirstOrDefault<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        TEntity Last(Expression<Func<TEntity, bool>> predicate);
        TResult Last<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate);
        TResult LastOrDefault<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        TResult Single<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        TResult SingleOrDefault<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        int Count();
        int Count(Expression<Func<TEntity, bool>> predicate);
        long LongCount();
        long LongCount(Expression<Func<TEntity, bool>> predicate);
        bool Any(Expression<Func<TEntity, bool>> predicate);
        bool All(Expression<Func<TEntity, bool>> predicate);
    }
    
    public abstract class EntityRepositoryBase<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity
    {
        public ICurrentUserProvider CurrentUserProvider { get; set; }

        private readonly IEntityContext _entityContext;
        protected readonly IQueryable<TEntity> Query;

        protected EntityRepositoryBase(IEntityContext entityContext, bool asNoTracking)
        {
            _entityContext = entityContext;
            Query = _entityContext.Set<TEntity>(asNoTracking);
        }

        public void Save(TEntity entity)
        {
            var currentUserId = entity.ModifiedByUserId = CurrentUserProvider.GetCurrentUser()?.Id;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedByUserId = currentUserId;

            if (entity.IsNew())
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedByUserId = currentUserId;
                _entityContext.Add(entity);
            }
            else
                _entityContext.Attach(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _entityContext.Remove(entity);
        }

        public TEntity GetById(long id)
        {
            return First(e => e.Id == id);
        }

        public TResult GetById<TResult>(long id, Expression<Func<TEntity, TResult>> selector)
        {
            return First(e => e.Id == id, selector);
        }

        public TEntity TryGetById(long id)
        {
            return FirstOrDefault(e => e.Id == id);
        }

        public TResult TryGetById<TResult>(long id, Expression<Func<TEntity, TResult>> selector)
        {
            return FirstOrDefault(e => e.Id == id, selector);
        }

        public IQueryable<TEntity> Select(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.Where(predicate);
        }

        public IQueryable<TResult> Select<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Where(predicate).Select(selector);
        }

        public IQueryable<TEntity> SelectAll()
        {
            return Query;
        }

        public IQueryable<TResult> SelectAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Select(selector);
        }

        public IQueryable<TResult> SelectMany<TResult>(Expression<Func<TEntity, IEnumerable<TResult>>> selector)
        {
            return Query.SelectMany(selector);
        }

        public IQueryable<TResult> SelectMany<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, IEnumerable<TResult>>> selector)
        {
            return Query.Where(predicate).SelectMany(selector);
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.First(predicate);
        }

        public TResult First<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Where(predicate).Select(selector).First();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.FirstOrDefault(predicate);
        }

        public TResult FirstOrDefault<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Where(predicate).Select(selector).FirstOrDefault();
        }

        public TEntity Last(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.Last(predicate);
        }

        public TResult Last<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Where(predicate).Select(selector).Last();
        }

        public TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.LastOrDefault(predicate);
        }

        public TResult LastOrDefault<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Where(predicate).Select(selector).LastOrDefault();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.Single(predicate);
        }

        public TResult Single<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Where(predicate).Select(selector).Single();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.SingleOrDefault(predicate);
        }

        public TResult SingleOrDefault<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Query.Where(predicate).Select(selector).SingleOrDefault();
        }

        public int Count()
        {
            return Query.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.Count(predicate);
        }

        public long LongCount()
        {
            return Query.LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.LongCount(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.FirstOrDefault(predicate) != null;
        }

        public bool All(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.All(predicate);
        }

        public IEnumerable<EntityChange<TEntity>> GetChanges()
        {
            return _entityContext.GetEntityChanges<TEntity>();
        }

        public IEnumerable<IEntityEvent<TEntity>> GetEntityEvents()
        {
            var changes = GetChanges();
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
