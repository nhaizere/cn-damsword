using DamSword.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DamSword.Web.Attributes
{
    public class RequireAttribute : ActionFilterAttribute
    {
        public UserPermissions Permissions { get; set; }

        public RequireAttribute(UserPermissions permissions)
        {
            Permissions = permissions;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            (context.Controller as Controller)?.Require(Permissions);
        }
    }
}