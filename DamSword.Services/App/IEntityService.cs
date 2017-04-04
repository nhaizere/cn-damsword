using DamSword.Common;
using DamSword.Data;

namespace DamSword.Services
{
    public interface IEntityService<TEntity> : IService
        where TEntity : IEntity
    {
        void Save(TEntity entity);
        void Delete(long id);
    }
}