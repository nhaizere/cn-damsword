using System;
using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IDataSnapshotRepository : IEntityRepository<DataSnapshot>
    {
        DataSnapshot GetOrFetchDataSnapshot(long webResourceId, long personId, DateTime date, int type);
    }

    public class DataSnapshotRepository : EntityRepositoryBase<DataSnapshot>, IDataSnapshotRepository
    {
        public DataSnapshotRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }

        public DataSnapshot GetOrFetchDataSnapshot(long webResourceId, long personId, DateTime date, int type)
        {
            var snapshotDate = date.Date;
            var snapshot = FirstOrDefault(s => s.WebResourceId == webResourceId && s.PersonId == personId && s.Date == snapshotDate && s.Type == type);
            if (snapshot != null)
                return snapshot;

            return new DataSnapshot
            {
                Date = snapshotDate,
                PersonId = personId,
                WebResourceId = webResourceId,
                Type = type
            };
        }
    }
}