using System;
using System.Collections.Generic;
using DamSword.Common;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Watch;
using DamSword.Web.Models.Migration;
using DamSword.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class MigrationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

        public MigrationController(IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Init()
        {
            var hasOwner = _userRepository.GetOwner() != null;
            return View("~/Views/Migration/Init.cshtml", new InitViewModel
            {
                IsInitialized = hasOwner
            });
        }

        [HttpPost]
        public IActionResult Init(InitSaveModel model)
        {
            var hasOwner = _userRepository.GetOwner() != null;
            if (hasOwner)
                throw new InvalidOperationException("Application is already initialized.");

            if (model.Alias?.Length < 5)
                throw new InvalidOperationException($"\"{nameof(model.Alias)}\" length must be at least 5 characters long.");
            if (model.Login?.Length < 5)
                throw new InvalidOperationException($"\"{nameof(model.Login)}\" length must be at least 5 characters long.");
            if (model.Password?.Length < 6)
                throw new InvalidOperationException($"\"{nameof(model.Password)}\" length must be at least 5 characters long.");

            _userRepository.Save(new User
            {
                Alias = model.Alias,
                Login = model.Login,
                PasswordHash = PasswordUtils.CreateHash(model.Password),
                Permissions = UserPermissions.Owner
            });

            var watchServices = ServiceLocator.Resolve<IEnumerable<IWatchService>>();
            foreach (var watchService in watchServices)
            {
                watchService.EnsureRegistered();
            }

            _unitOfWork.Commit();

            var isLoggedIn = _authenticationService.TryLogIn(model.Login, model.Password, false, HttpContext);
            if (!isLoggedIn)
                throw new InvalidOperationException("Unable to perform Authentication for Owner.");

            return RedirectToAction("Index", "Home");
        }
    }
}