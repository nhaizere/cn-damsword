using System.Text.Encodings.Web;
using DamSword.Common;
using DamSword.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DamSword.Web.Attributes
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionService = ServiceLocator.Resolve<ISessionService>();
            var sessionHash = context.HttpContext.Request.Cookies["session"];
            var remoteIpAddress = context.HttpContext.GetRemoteIpAddress();
            var session = sessionService.GetSession(sessionHash, remoteIpAddress);

            if (session == null)
            {
                var returnUrl = context.HttpContext.Request.GetUri().PathAndQuery;
                var returnUrlEncoded = returnUrl == "/" ? string.Empty : UrlEncoder.Default.Encode(context.HttpContext.Request.GetUri().PathAndQuery);
                context.Result = new RedirectResult($"/account/login{(returnUrlEncoded.IsEmpty() ? string.Empty : $"?returnUrl={returnUrlEncoded}")}");
            }
        }
    }
}