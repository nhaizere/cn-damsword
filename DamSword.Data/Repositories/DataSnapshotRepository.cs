using System;
using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IDataSnapshotRepository : IEntityRepository<DataSnapshot>
    {
        DataSnapshot GetDataSnapshot(long webResourceId, long personId, DateTime date, int type);
    }

    public class DataSnapshotRepository : EntityRepositoryBase<DataSnapshot>, IDataSnapshotRepository
    {
        public DataSnapshotRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }

        public DataSnapshot GetDataSnapshot(long webResourceId, long personId, DateTime date, int type)
        {
            return FirstOrDefault(s => s.WebResourceId == webResourceId && s.PersonId == personId && s.Date == date.Date && s.Type == type);
        }
    }
}