using DamSword.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DamSword.Web.Filters
{
    public class AjaxExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!context.HttpContext.Request.IsAjaxRequest())
                return;

            var statusCode = (int?) (context.Exception as RequestException)?.StatusCode ?? 500;

            context.HttpContext.Response.StatusCode = statusCode;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new JsonResult(new
            {
                StatusCode = statusCode,
                context.Exception.Message
            });
        }
    }
}