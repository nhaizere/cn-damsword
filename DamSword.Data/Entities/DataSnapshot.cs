﻿using System;

namespace DamSword.Data.Entities
{
    public class DataSnapshot : EntityBase
    {
        public DateTime Date { get; set; }
        public int PersonId { get; set; }
        public int WebResourceId { get; set; }
        public byte[] Data { get; set; }
        
        public virtual Person Person { get; set; }
        public virtual WebResource WebResource { get; set; }
    }
}