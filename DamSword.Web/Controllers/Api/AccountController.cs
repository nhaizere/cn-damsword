using DamSword.Web.Attributes;
using DamSword.Web.DTO;
using DamSword.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers.Api
{
    public class AccountController : ApiControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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
    }
}