using DamSword.Common;
using DamSword.Data.Repositories;

namespace DamSword.Services
{
    public interface IEventLogService : IService
    {
    }

    public class EventLogService : IEventLogService
    {
        public IEventLogRepository EventLogRepository { get; set; }
    }
}