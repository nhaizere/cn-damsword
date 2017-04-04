using System;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Services.Entity
{
    public class SessionInfo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Hash { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

    public interface ISessionService : IEntityService<Session>
    {
        void ExtendSession(long id, TimeSpan time);
        void RemoveSession(long id);
        SessionInfo GetSession(string hash, string remoteIpAddress);
        SessionInfo CreateSession(long userId, string remoteIpAddress, bool persistent);
    }

    public class SessionService : ISessionService
    {
        public ISessionRepository SessionRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }

        public void Save(Session entity)
        {
            SessionRepository.Save(entity);
        }

        public void Delete(long id)
        {
            SessionRepository.Delete(id);
        }

        public void ExtendSession(long id, TimeSpan time)
        {
            var session = SessionRepository.GetById(id);
            session.ExpirationTime += time;

            Save(session);
            UnitOfWork.Commit();
        }

        public void RemoveSession(long id)
        {
            var session = SessionRepository.GetById(id);
            session.ExpirationTime = DateTime.UtcNow;

            Save(session);
            UnitOfWork.Commit();
        }

        public SessionInfo GetSession(string hash, string remoteIpAddress)
        {
            return SessionRepository.FirstOrDefault(s => s.SessionHash == hash && s.RemoteIpAddress == remoteIpAddress && s.ExpirationTime > DateTime.UtcNow, s => new SessionInfo
            {
                Id = s.Id,
                UserId = s.UserId,
                Hash = hash,
                ExpirationTime = s.ExpirationTime
            });
        }

        public SessionInfo CreateSession(long userId, string remoteIpAddress, bool persistent)
        {
            if (!UserRepository.Any(u => u.Id == userId))
                throw new InvalidOperationException("User doesn't exist.");

            var hash = Guid.NewGuid().ToString();
            var now = DateTime.Now;
            var session = new Session
            {
                UserId = userId,
                SessionHash = hash,
                RemoteIpAddress = remoteIpAddress,
                ExpirationTime = persistent ? now.AddMonths(1) : now.AddDays(1)
            };

            Save(session);
            UnitOfWork.Commit();

            return new SessionInfo
            {
                Id = session.Id,
                UserId = userId,
                Hash = hash,
                ExpirationTime = session.ExpirationTime
            };
        }
    }
}