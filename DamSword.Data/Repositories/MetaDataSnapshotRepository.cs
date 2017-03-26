﻿using System;
using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IMetaDataSnapshotRepository : IEntityRepository<MetaDataSnapshot>
    {
        MetaDataSnapshot GetOrFetchDataSnapshot(long providerId, long personId, DateTime date, int snapshotType);
    }

    public class MetaDataSnapshotRepository : EntityRepositoryBase<MetaDataSnapshot>, IMetaDataSnapshotRepository
    {
        public MetaDataSnapshotRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }

        public MetaDataSnapshot GetOrFetchDataSnapshot(long providerId, long personId, DateTime date, int snapshotType)
        {
            var snapshotDate = date.Date;
            var snapshot = FirstOrDefault(s => s.ProviderId == providerId && s.PersonId == personId && s.Date == snapshotDate && s.SnapshotType == snapshotType);
            if (snapshot != null)
                return snapshot;

            return new MetaDataSnapshot
            {
                PersonId = personId,
                ProviderId = providerId,
                Date = snapshotDate,
                SnapshotType = snapshotType
            };
        }
    }
}