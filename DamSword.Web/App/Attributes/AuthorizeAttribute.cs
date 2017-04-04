using System;
using DamSword.Common;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DamSword.Web.Attributes
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!CurrentUser.IsAnonymous)
                return;

            if (context.HttpContext.Request.IsApiRequest())
                throw new UnauthorizedAccessException();

            var returnUrl = context.HttpContext.Request.GetUri().PathAndQuery;
            if (returnUrl == "/")
                returnUrl = null;

            // TODO: encode return URL
            context.Result = new RedirectResult($"/account/login{(returnUrl.IsNullOrEmpty() ? string.Empty : $"?returnUrl={returnUrl}")}");
        }
    }
}