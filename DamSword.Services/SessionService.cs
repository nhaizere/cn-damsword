using System;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services.App;

namespace DamSword.Services
{
    public interface ISessionService
    {
        string GetNewSessionHash(long userId, string ipAddress, bool persistent);
        void ExtendSession(int id, TimeSpan time);
        void RemoveSession(int id);
        long? GetSessionUserId(string hash, string ipAddress);
    }

    public class SessionService : IService, ISessionService
    {
        public ISessionRepository SessionRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }

        public string GetNewSessionHash(long userId, string ipAddress, bool persistent)
        {
            if (!UserRepository.Any(u => u.Id == userId))
                throw new InvalidOperationException("User doesn't exist.");

            var hash = Guid.NewGuid().ToString();
            var now = DateTime.Now;
            var session = new Session
            {
                UserId = userId,
                SessionHash = hash,
                RemoteIpAddress = ipAddress,
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

        public long? GetSessionUserId(string hash, string ipAddress)
        {
            return SessionRepository.FirstOrDefault(s => s.SessionHash == hash && s.RemoteIpAddress == ipAddress && s.ExpirationTime <= DateTime.UtcNow, s => s.UserId);
        }
    }
}