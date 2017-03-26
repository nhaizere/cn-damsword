using System;

namespace DamSword.Data.Entities
{
    public class MetaDataSnapshot : MetaEntityBase
    {
        public DateTime Date { get; set; }
        public int SnapshotType { get; set; }
        public byte[] Data { get; set; }
    }
}