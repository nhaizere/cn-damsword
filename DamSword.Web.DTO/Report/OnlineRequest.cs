using System;
using System.Collections.Generic;

namespace DamSword.Web.DTO.Report
{
    public class OnlineRequest
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public IEnumerable<long> PersonIds { get; set; }
    }
}