using System;
using System.Net;

namespace DamSword.Web.Exceptions
{
    public class RequestException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public RequestException(HttpStatusCode statusCode, string message = null)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}