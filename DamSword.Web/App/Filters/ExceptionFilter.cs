﻿using DamSword.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DamSword.Web.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (CommonAppConfig.IsDebug)
                return;

            var result = new ViewResult
            {
                ViewName = "~/Views/Error/Details.cshtml",
                ViewData= new ViewDataDictionary<Models.Error.DetailsModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = new Models.Error.DetailsModel
                    {
                        StatusCode = 500,
                        Exception = context.Exception
                    }
                }
            };

            context.Result = result;
        }
    }
}