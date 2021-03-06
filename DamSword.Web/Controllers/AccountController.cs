﻿using DamSword.Common;
using DamSword.Web.Models.Account;
using DamSword.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            // TODO: implement smart anti-XSS logic
            if (returnUrl.NonNullOrEmpty() && !returnUrl.StartsWith("/"))
                returnUrl = "/";

            return View("~/Views/Account/Login.cshtml", new LoginViewModel
            {
                InvalidCredentials = false,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult Login(LoginSaveModel model)
        {
            var isLoggedIn = _authenticationService.TryLogIn(model.Login, model.Password, model.Persistent, HttpContext);
            if (!isLoggedIn)
            {
                return View("~/Views/Account/Login.cshtml", new LoginViewModel
                {
                    InvalidCredentials = true,
                    ReturnUrl = model.ReturnUrl
                });
            }

            return Redirect(model.ReturnUrl ?? "/");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            if (SessionScope.Current.Session != null)
                _authenticationService.LogOut(SessionScope.Current.Session.Id, HttpContext);

            return RedirectToAction("Login");
        }
    }
}