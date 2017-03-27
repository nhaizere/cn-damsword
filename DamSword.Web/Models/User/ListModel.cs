using System.Collections.Generic;
using DamSword.Data.Entities;

namespace DamSword.Web.Models.User
{
    public class ListModel
    {
        public string Search { get; set; }
        public IEnumerable<User> Users { get; set; }

        public class User
        {
            public long Id { get; set; }
            public string Alias { get; set; }
            public string Login { get; set; }
            public int HierarchyLevel { get; set; }
            public UserPermissions Permissions { get; set; }
        }
    }
}