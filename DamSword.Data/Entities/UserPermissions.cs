using System;

namespace DamSword.Data.Entities
{
    [Flags]
    public enum UserPermissions : long
    {
        Owner = 1 << 31
    }
}