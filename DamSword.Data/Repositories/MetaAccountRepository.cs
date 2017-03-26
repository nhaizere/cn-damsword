using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IMetaAccountRepository : IEntityRepository<MetaAccount>
    {
    }

    public class MetaAccountRepository : EntityRepositoryBase<MetaAccount>, IMetaAccountRepository
    {
        public MetaAccountRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}