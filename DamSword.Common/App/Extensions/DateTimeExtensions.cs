using System;

namespace DamSword.Common
{
    public static class DateTimeExtensions
    {
        public static DateTime FromUnixTimeStamp(long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(timestamp);
        }
    }
}