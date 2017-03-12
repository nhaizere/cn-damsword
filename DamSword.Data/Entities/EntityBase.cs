using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DamSword.Data.Entities
{
    public abstract class EntityBase : IEntity
    {
        [Key]
        public long Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
        
        public long CreatedByUserId { get; set; }
        public long ModifiedByUserId { get; set; }
    }
}