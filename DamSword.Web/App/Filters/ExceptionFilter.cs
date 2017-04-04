using System;
using System.Net;
using DamSword.Common;
using DamSword.Web.DTO;
using DamSword.Web.Exceptions;
using DamSword.Web.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace DamSword.Web.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = context.HttpContext.Request.IsApiRequest() && context.HttpContext.Request.IsJsonRequest()
                ? GetApiErrorResult(context)
                : GetWebErrorResult(context);

            context.ExceptionHandled = true;
        }

        private static IActionResult GetApiErrorResult(ExceptionContext context)
        {
            var statusCode = GetStatusCodeFromException(context.Exception);
            var routeData = context.HttpContext.GetRouteData();

            // TODO: fix IRequest resolver ()
            var request = routeData?.Values["request"] as IRequest ?? new EmptyRequest();
            var response = new Response<Error>(request, new Error
            {
                StatusCode = statusCode,
                Message = context.Exception.Message
            }, false);

            var responseJson = response.ToJson();
            return new ContentResult
            {
                Content = responseJson,
                ContentType = "application/json",
                StatusCode = statusCode
            };
        }

        private static IActionResult GetWebErrorResult(ExceptionContext context)
        {
            var statusCode = GetStatusCodeFromException(context.Exception);
            if (context.HttpContext.Request.IsAjaxRequest() && context.HttpContext.Request.IsJsonRequest())
            {
                var model = new AjaxErrorModel { Message = context.Exception.Message };
                var responseJson = model.ToJson();
                return new ContentResult
                {
                    Content = responseJson,
                    ContentType = "application/json",
                    StatusCode = statusCode
                };
            }
            
            return new ViewResult
            {
                ViewName = "~/Views/Error/Details.cshtml",
                ViewData = new ViewDataDictionary<Models.Error.DetailsModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new Models.Error.DetailsModel
                    {
                        StatusCode = statusCode,
                        Exception = context.Exception
                    }
                }
            };
        }

        private static int GetStatusCodeFromException(Exception exception)
        {
            switch (exception)
            {
                case UnauthorizedAccessException e:
                    return (int)HttpStatusCode.Unauthorized;
                case RequestException e:
                    return (int)e.StatusCode;
                default:
                    return (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}