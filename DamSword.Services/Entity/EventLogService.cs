using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Services
{
    public interface IEventLogService : IEntityService<EventLog>
    {
    }

    public class EventLogService : IEventLogService
    {
        public IEventLogRepository EventLogRepository { get; set; }

        public void Save(EventLog entity)
        {
            EventLogRepository.Save(entity);
        }

        public void Delete(long id)
        {
            EventLogRepository.Delete(id);
        }
    }
}