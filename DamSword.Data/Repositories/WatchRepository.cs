using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IWatchRepository : IEntityRepository<Watch>
    {
    }

    public class WatchRepository : EntityRepositoryBase<Watch>, IWatchRepository
    {
        public WatchRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}