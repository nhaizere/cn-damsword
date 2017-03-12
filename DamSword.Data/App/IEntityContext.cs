using System.Linq;

namespace DamSword.Data
{
    public interface IEntityContext : IEntityChangesProvider
    {
        void Add<TEntity>(TEntity entity)
            where TEntity : class, IEntity;
        void Attach<TEntity>(TEntity entity)
            where TEntity : class, IEntity;
        void Remove<TEntity>(TEntity entity)
            where TEntity : class, IEntity;
        IQueryable<TEntity> Set<TEntity>(bool asNoTracking)
            where TEntity : class, IEntity;
        
        void SaveChanges();
    }
}
