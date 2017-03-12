using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IUserRepository : IEntityRepository<User>
    {
    }

    public class UserRepository : EntityRepositoryBase<User>, IUserRepository
    {
        public UserRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}