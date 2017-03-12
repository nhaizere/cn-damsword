using System;

namespace DamSword.Data
{
    public interface IEntity
    {
        long Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }
        long? CreatedByUserId { get; set; }
        long? ModifiedByUserId { get; set; }
    }
}