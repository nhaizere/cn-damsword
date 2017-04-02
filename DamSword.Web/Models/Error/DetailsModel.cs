using System;
using System.Net;
using DamSword.Common;

namespace DamSword.Web.Models.Error
{
    public class DetailsModel
    {
        public int StatusCode { get; set; }
        public Exception Exception { get; set; }

        public string StatusCodeMessage
        {
            get
            {
                if (!Enum.IsDefined(typeof(HttpStatusCode), StatusCode))
                    return "Unknown Error";

                var httpStatusCode = (HttpStatusCode)StatusCode;
                return httpStatusCode.ToString().SplitUpperCase().Join(" ");
            }
        }
    }
}