using DamSword.Data.Repositories;

namespace DamSword.Data
{
    public static class EntityRepositoryExtensions
    {
        public static void Delete<TEntity>(this IEntityRepository<TEntity> self, long id)
            where TEntity : IEntity
        {
            var entity = self.GetById(id);
            self.Delete(entity);
        }
    }
}
