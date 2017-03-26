using System;

namespace DamSword.Watch.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum FromUnderlyingType<TEnum>(object value, TEnum defaultValue)
            where TEnum : struct
        {
            return value != null && Enum.IsDefined(typeof(TEnum), value) ? (TEnum)value : defaultValue;
        }

        public static TEnum FromString<TEnum>(string value, TEnum defaultValue)
            where TEnum : struct
        {
            TEnum type;
            return Enum.TryParse(value, out type) ? type : defaultValue;
        }
    }
}