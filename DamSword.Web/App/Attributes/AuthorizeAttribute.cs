using DamSword.Common;
using DamSword.Data.Entities;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DamSword.Web.Attributes
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public UserPermissions Require { get; set; }

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

            if (!HasPermissions())
                context.Result = new UnauthorizedResult();
        }

        private bool HasPermissions()
        {
            if (Require == UserPermissions.None || UserScope.Current?.User.Permissions == UserPermissions.Owner)
                return true;
            
            return Permissions.Has(Require);
        }
    }
}