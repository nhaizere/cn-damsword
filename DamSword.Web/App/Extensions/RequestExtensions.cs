using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

namespace DamSword.Web
{
    public static class RequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            
            if (self.Headers != null)
                return self.Headers["X-Requested-With"] == "XMLHttpRequest";

            return false;
        }

        public static bool IsJsonRequest(this HttpRequest self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return self.Headers["Accept"].Contains("application/json");
        }

        public static bool IsApiRequest(this HttpRequest self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            var path = self.GetUri().LocalPath;
            return path.StartsWith("/api/");
        }
    }
}