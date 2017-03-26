using System;
using DamSword.Data.Repositories;
using DamSword.Services;
using Microsoft.AspNetCore.Http;

namespace DamSword.Web.Services
{
    public interface IAuthenticationService : IService
    {
        bool TryLogIn(string login, string password, bool persistent, HttpContext context);
        void LogOut(long sessionId, HttpContext context);
        string GetCurrentSessionHash(HttpContext context);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionService _sessionService;

        private const string SessionCookieName = "SOD_SESSION_IDENTITY";

        public AuthenticationService(IUserRepository userRepository, ISessionService sessionService)
        {
            _userRepository = userRepository;
            _sessionService = sessionService;
        }

        public bool TryLogIn(string login, string password, bool persistent, HttpContext context)
        {
            var passwordHash = PasswordUtils.CreateHash(password);
            var userId = _userRepository.FirstOrDefault(u => u.Login == login && u.PasswordHash == passwordHash, u => u.Id);
            if (userId == 0)
                return false;

            var remoteIpAddress = context.GetRemoteIpAddress();
            var session = _sessionService.CreateSession(userId, remoteIpAddress, persistent);
            context.Response.Cookies.Append(SessionCookieName, session.Hash, new CookieOptions
            {
                Expires = new DateTimeOffset(session.ExpirationTime)
            });

            return true;
        }

        public void LogOut(long sessionId, HttpContext context)
        {
            _sessionService.RemoveSession(sessionId);
            context.Response.Cookies.Delete(SessionCookieName);
        }

        public string GetCurrentSessionHash(HttpContext context)
        {
            return context.Request.Cookies[SessionCookieName];
        }
    }
}