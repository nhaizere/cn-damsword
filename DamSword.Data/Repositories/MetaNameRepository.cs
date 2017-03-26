using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IMetaNameRepository : IEntityRepository<MetaName>
    {
    }

    public class MetaNameRepository : EntityRepositoryBase<MetaName>, IMetaNameRepository
    {
        public MetaNameRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}