using System;

namespace DamSword.Common
{
    public static class DateTimeExtensions
    {
        private static readonly long EpochTicks = new DateTime(1970, 1, 1).Ticks;

        public static DateTime FromUnixTimeStamp(long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(timestamp);
        }

        public static long ToUnixTimeStamp(this DateTime self)
        {
            return (self.Ticks - EpochTicks) / TimeSpan.TicksPerSecond;
        }
    }
}