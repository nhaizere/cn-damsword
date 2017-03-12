using System;
using System.ComponentModel.DataAnnotations;

namespace DamSword.Data.Entities
{
    public class Session : EntityBase
    {
        public long UserId { get; set; }

        [Required]
        public string SessionHash { get; set; }

        [Required]
        public string RemoteIpAddress { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}