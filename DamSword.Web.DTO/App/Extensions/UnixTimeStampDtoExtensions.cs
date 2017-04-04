using System;
using DamSword.Common;

namespace DamSword.Web.DTO
{
    public static class UnixTimeStampDtoExtensions
    {
        public static DateTime GetTimeStamp(this IUnixTimeStampDto self)
        {
            return DateTimeExtensions.FromUnixTimeStamp(self.UnixTimeStamp);
        }

        public static void SetTimeStamp(this IUnixTimeStampDto self, DateTime timestamp)
        {
            self.UnixTimeStamp = timestamp.ToUnixTimeStamp();
        }
    }
}