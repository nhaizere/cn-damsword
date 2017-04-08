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
    }
}