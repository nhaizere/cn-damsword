using System.Collections.Generic;
using DamSword.Common;
using DamSword.Web.Controllers.Api;
using DamSword.Web.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web
{
    public static class ApiControllerExtensions
    {
        public static IActionResult JsonResult(this ApiControllerBase self, object data)
        {
            var json = data.ToJson();
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        public static IActionResult ApiResult<TData>(this ApiControllerBase self, IRequest request, TData data)
        {
            var result = new Response<TData>(request, data);
            return self.JsonResult(result);
        }

        public static IActionResult ApiListResult<TItem>(this ApiControllerBase self, IRequest request, IEnumerable<TItem> items)
        {
            return self.ApiResult(request, new GenericListResult<TItem>(items));
        }

        public static IActionResult ApiSuccessResult(this ApiControllerBase self, IRequest request)
        {
            var result = new EmptyRespone(request, isSuccessful: true);
            return self.JsonResult(result);
        }

        public static IActionResult ApiFailResult(this ApiControllerBase self, IRequest request)
        {
            var result = new EmptyRespone(request, isSuccessful: false);
            return self.JsonResult(result);
        }
    }
}