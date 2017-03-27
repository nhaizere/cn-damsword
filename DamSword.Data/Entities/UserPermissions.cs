using System;

namespace DamSword.Data.Entities
{
    [Flags]
    public enum UserPermissions : long
    {
        None = 0,
        ViewUsers = 1 << 0,
        ManageUsers = 1 << 1,
        ViewPersons = 1 << 10,
        ManagePersons = 1 << 11,
        ViewPersonOnlineMeta = 1 << 12,
        ViewPersonDataMeta = 1 << 13,
        ViewWatches = 1 << 20,
        ManageWatches = 1 << 21,
        TheQueen = 1 << 30,
        Owner = 1 << 31
    }
}