using System;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Services
{
    public class SessionInfo
    {
        public long Id { get; set; }
        public long UserId { get; set; }
    }

    public interface ISessionService
    {
        string GetNewSessionHash(long userId, string remoteIpAddress, bool persistent);
        void ExtendSession(int id, TimeSpan time);
        void RemoveSession(int id);
        SessionInfo GetSession(string hash, string remoteIpAddress);
    }

    public class SessionService : IService, ISessionService
    {
        public ISessionRepository SessionRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }

        public string GetNewSessionHash(long userId, string remoteIpAddress, bool persistent)
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

            SessionRepository.Save(session);
            UnitOfWork.Commit();

            return hash;
        }

        public void ExtendSession(int id, TimeSpan time)
        {
            var session = SessionRepository.GetById(id);
            session.ExpirationTime += time;

            SessionRepository.Save(session);
            UnitOfWork.Commit();
        }

        public void RemoveSession(int id)
        {
            var session = SessionRepository.GetById(id);
            session.ExpirationTime = DateTime.UtcNow;

            SessionRepository.Save(session);
            UnitOfWork.Commit();
        }

        public SessionInfo GetSession(string hash, string remoteIpAddress)
        {
            return SessionRepository.FirstOrDefault(s => s.SessionHash == hash && s.RemoteIpAddress == remoteIpAddress && s.ExpirationTime > DateTime.UtcNow, s => new SessionInfo
            {
                Id = s.Id,
                UserId = s.UserId
            });
        }
    }
}