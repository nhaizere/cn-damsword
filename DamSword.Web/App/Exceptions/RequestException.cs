using System;

namespace DamSword.Web.Exceptions
{
    public class RequestException : Exception
    {
        public int StatusCode { get; }

        public RequestException(int statusCode, string message = null)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}