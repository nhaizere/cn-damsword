using System;
using System.Collections.Generic;

namespace DamSword.Web.DTO.Report
{
    public class OnlineResponse
    {
        public IEnumerable<OnlineSnapshot> Snapshots { get; set; }

        public class OnlineSnapshot
        {
            public DateTime From { get; set; }
            public long PersonId { get; set; }
            public long WebResourceId { get; set; }
            public string AccountId { get; set; }
            public IEnumerable<TimelineChunk> Chunks { get; set; }
        }

        public class TimelineChunk
        {
            public int? OnlineType { get; set; }
            public string OnlineMeta { get; set; }
            public TimeSpan Length { get; set; }
        }
    }
}