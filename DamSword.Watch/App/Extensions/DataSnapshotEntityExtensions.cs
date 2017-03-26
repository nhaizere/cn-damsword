using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamSword.Data.Entities;
using Newtonsoft.Json;

namespace DamSword.Watch.Extensions
{
    public static class DataSnapshotEntityExtensions
    {
        public static IEnumerable<TDataSnapshotValue> GetSnapshots<TDataSnapshotValue>(this MetaDataSnapshot self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (self.Data == null || self.Data.Length == 0)
                return Enumerable.Empty<TDataSnapshotValue>();

            var json = Encoding.UTF8.GetString(self.Data);
            return JsonConvert.DeserializeObject<IEnumerable<TDataSnapshotValue>>(json);
        }

        public static void SetSnapshots<TDataSnapshotValue>(this MetaDataSnapshot self, IEnumerable<TDataSnapshotValue> snapshots)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (snapshots == null)
                throw new ArgumentNullException(nameof(snapshots));

            var json = JsonConvert.SerializeObject(snapshots);
            self.Data = Encoding.UTF8.GetBytes(json);
        }
    }
}