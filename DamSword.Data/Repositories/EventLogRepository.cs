using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IEventLogRepository : IEntityRepository<EventLog>
    {
    }

    public class EventLogRepository : EntityRepositoryBase<EventLog>, IEventLogRepository
    {
        public EventLogRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}