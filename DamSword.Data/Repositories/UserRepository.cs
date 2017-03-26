using System;
using System.Linq.Expressions;
using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IUserRepository : IEntityRepository<User>
    {
        User GetOwner();
        TResult GetOwner<TResult>(Expression<Func<User, TResult>> selector);
    }

    public class UserRepository : EntityRepositoryBase<User>, IUserRepository
    {
        public UserRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }

        public User GetOwner()
        {
            return GetOwner(u => u);
        }

        public TResult GetOwner<TResult>(Expression<Func<User, TResult>> selector)
        {
            return FirstOrDefault(u => (u.Permissions & UserPermissions.Owner) != 0, selector);
        }
    }
}