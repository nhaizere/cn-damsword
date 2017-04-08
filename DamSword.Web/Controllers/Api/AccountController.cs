using System;
using System.Collections.Generic;
using System.Net;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services.Entity;
using DamSword.Watch;
using DamSword.Web.Attributes;
using DamSword.Web.DTO;
using DamSword.Web.Exceptions;
using DamSword.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers.Api
{
    public class AccountController : ApiControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IWatch> _watches;

        public AccountController(IAuthenticationService authenticationService, IUserRepository userRepository, IUserService userService, IUnitOfWork unitOfWork, IEnumerable<IWatch> watches)
        {
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _watches = watches;
        }

        [HttpPost, AjaxOnly]
        public IActionResult Login([FromBody] Request<Login> request)
        {
            var isLoggedIn = _authenticationService.TryLogIn(request.Data.Name, request.Data.Password, request.Data.Persistent, HttpContext);
            return isLoggedIn
                ? this.ApiSuccessResult(request)
                : this.ApiFailResult(request);
        }
        
        [HttpPost, AjaxOnly]
        public IActionResult Logout([FromBody] EmptyRequest request)
        {
            if (SessionScope.Current.Session == null)
                this.ApiFailResult(request);

            _authenticationService.LogOut(SessionScope.Current.Session.Id, HttpContext);
            return this.ApiSuccessResult(request);
        }

        [HttpPost, AjaxOnly]
        [Route("/api/system/initialization")]
        public IActionResult Initialize([FromBody] Request<Initialize> request)
        {
            var hasOwner = _userRepository.GetOwner() != null;
            if (hasOwner)
                throw new RequestException(HttpStatusCode.NotFound);

            if (request.Data.Alias?.Length < 5)
                throw new InvalidOperationException($"\"{nameof(request.Data.Alias)}\" length must be at least 5 characters long.");
            if (request.Data.Login?.Length < 5)
                throw new InvalidOperationException($"\"{nameof(request.Data.Login)}\" length must be at least 5 characters long.");
            if (request.Data.Password?.Length < 6)
                throw new InvalidOperationException($"\"{nameof(request.Data.Password)}\" length must be at least 5 characters long.");

            _userService.Save(new User
            {
                Alias = request.Data.Alias,
                Login = request.Data.Login,
                PasswordHash = PasswordUtils.CreateHash(request.Data.Password),
                HierarchyLevel = 0,
                Permissions = UserPermissions.Owner
            });
            
            foreach (var watchService in _watches)
            {
                watchService.EnsureRegistered();
            }

            _unitOfWork.Commit();

            var isLoggedIn = _authenticationService.TryLogIn(request.Data.Login, request.Data.Password, false, HttpContext);
            if (!isLoggedIn)
                throw new InvalidOperationException("Unable to perform Authentication for Owner.");

            return this.ApiSuccessResult(request);
        }
    }
}