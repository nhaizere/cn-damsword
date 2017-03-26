using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IDataSnapshotRepository : IEntityRepository<DataSnapshot>
    {
    }

    public class DataSnapshotRepository : EntityRepositoryBase<DataSnapshot>, IDataSnapshotRepository
    {
        public DataSnapshotRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}