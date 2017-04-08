using System;
using System.Collections.Generic;

namespace DamSword.Watch
{
    public class OnlineTimeline
    {
        public DateTime From { get; set; }
        public long PersonId { get; set; }
        public string AccountId { get; set; }
        public IEnumerable<Chunk> Chunks { get; set; }

        public class Chunk
        {
            public int? OnlineType { get; set; }
            public string OnlineMeta { get; set; }
            public TimeSpan Length { get; set; }
        }
    }
}