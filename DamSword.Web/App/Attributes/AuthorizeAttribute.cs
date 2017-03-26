using DamSword.Common;
using DamSword.Services;
using DamSword.Web.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DamSword.Web.Attributes
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var authenticationService = ServiceLocator.Resolve<IAuthenticationService>();
            var sessionService = ServiceLocator.Resolve<ISessionService>();
            var sessionHash = authenticationService.GetCurrentSessionHash(context.HttpContext);
            var remoteIpAddress = context.HttpContext.GetRemoteIpAddress();
            var session = sessionService.GetSession(sessionHash, remoteIpAddress);

            if (session == null)
            {
                var returnUrl = context.HttpContext.Request.GetUri().PathAndQuery;

                // TODO: encode return URL
                var returnUrlEncoded = returnUrl == "/" ? string.Empty : context.HttpContext.Request.GetUri().PathAndQuery;
                context.Result = new RedirectResult($"/account/login{(returnUrlEncoded.IsEmpty() ? string.Empty : $"?returnUrl={returnUrlEncoded}")}");
            }
        }
    }
}