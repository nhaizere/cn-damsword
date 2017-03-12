using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface ISessionRepository : IEntityRepository<Session>
    {
    }

    public class SessionRepository : EntityRepositoryBase<Session>, ISessionRepository
    {
        public SessionRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}