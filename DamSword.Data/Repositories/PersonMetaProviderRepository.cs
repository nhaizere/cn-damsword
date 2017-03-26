using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IPersonMetaProviderRepository : IEntityRepository<PersonMetaProvider>
    {
    }

    public class PersonMetaProviderRepository : EntityRepositoryBase<PersonMetaProvider>, IPersonMetaProviderRepository
    {
        public PersonMetaProviderRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}