using System;
using DamSword.Common;
using DamSword.Data.Repositories;
using DamSword.Services;
using DamSword.Web.Models.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionService _sessionService;

        public AccountController(IUserRepository userRepository, ISessionService sessionService)
        {
            _userRepository = userRepository;
            _sessionService = sessionService;
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            // TODO: implement smart anti-XSS logic
            if (returnUrl.NonNullOrEmpty() && !returnUrl.StartsWith("/"))
                returnUrl = "/";

            return View("~/Views/Account/Login.cshtml", new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public ActionResult Login(LoginSaveModel model)
        {
            var passwordHash = PasswordUtils.CreateHash(model.Password);
            var userId = _userRepository.FirstOrDefault(u => u.Login == model.Login && u.PasswordHash == passwordHash, u => u.Id);
            if (userId == 0)
                return RedirectToAction("Login", new { returnUrl = model.ReturnUrl });

            var remoteIpAddress = HttpContext.GetRemoteIpAddress();
            var session = _sessionService.CreateSession(userId, remoteIpAddress, model.Persistent);
            Response.Cookies.Append("session", session.Hash, new CookieOptions
            {
                Expires = new DateTimeOffset(session.ExpirationTime)
            });

            return Redirect(model.ReturnUrl ?? "/");
        }
    }
}