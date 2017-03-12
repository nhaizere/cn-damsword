using DamSword.Common;
using Microsoft.AspNetCore.Http;

namespace DamSword.Web
{
    public static class RequestExtensions
    {
        public static string GetRemoteIpAddress(this HttpContext self)
        {
            var forwardedIpAddress = self.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString();
            return forwardedIpAddress.IsNullOrEmpty()
                ? self.Connection.RemoteIpAddress.ToString()
                : forwardedIpAddress.Split(',')[0];
        }
    }
}