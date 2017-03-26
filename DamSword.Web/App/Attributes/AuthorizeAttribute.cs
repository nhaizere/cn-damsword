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
            var user = UserScope.Current.User;
            if (user == null)
            {
                var returnUrl = context.HttpContext.Request.GetUri().PathAndQuery;

                // TODO: encode return URL
                var returnUrlEncoded = returnUrl == "/" ? string.Empty : context.HttpContext.Request.GetUri().PathAndQuery;
                context.Result = new RedirectResult($"/account/login{(returnUrlEncoded.IsEmpty() ? string.Empty : $"?returnUrl={returnUrlEncoded}")}");
            }
        }
    }
}