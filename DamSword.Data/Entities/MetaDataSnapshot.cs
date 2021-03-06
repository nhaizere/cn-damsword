﻿using System;

namespace DamSword.Data.Entities
{
    public class MetaDataSnapshot : MetaEntityBase
    {
        public string AccountId { get; set; }
        public DateTime Date { get; set; }
        public int SnapshotType { get; set; }
        public byte[] Data { get; set; }
    }
}