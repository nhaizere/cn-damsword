using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DamSword.Common
{
    public static class JsonExtensions
    {
        public static string ToJson(this object self, bool useCamelCasePropertyNamesContractResolver = false)
        {
            return useCamelCasePropertyNamesContractResolver
                ? JsonConvert.SerializeObject(self, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
                : JsonConvert.SerializeObject(self);
        }

        public static string ToJson(this object self, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(self, settings);
        }

        public static object FromJson(this string self, Type type)
        {
            if (type == typeof(string))
                return self;
            if (type == typeof(DateTime))
                return DateTime.ParseExact(self.Trim('"'), new[] { "dd.MM.yyyy" }, new CultureInfo("ru-RU"), DateTimeStyles.None);
            if (type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, self);


            return JsonConvert.DeserializeObject(self, type, new JsonSerializerSettings { DateFormatString = "dd.MM.yyyy" });
        }

        public static TResult FromJson<TResult>(this string self)
        {
            return (TResult)self.FromJson(typeof(TResult));
        }

        public static object FromJson(this string self)
        {
            return self.FromJson<object>();
        }

        public static TResult MapTo<TResult>(this object self)
        {
            return self.ToJson().FromJson<TResult>();
        }

        public static object MapTo(this object self, Type type)
        {
            return self.ToJson().FromJson(type);
        }

        public static T Clone<T>(this T self)
        {
            return self.MapTo<T>();
        }

        public static object ToObject<T1, T2>(this IDictionary<T1, T2> self)
        {
            return self.MapTo<object>();
        }

        public static object ToType(this object self, Type type)
        {
            return self?.MapTo(type);
        }

        public static TResult ToType<TResult>(this object self)
        {
            return self == null ? default(TResult) : self.MapTo<TResult>();
        }

        public static object ToType<T1, T2>(this IDictionary<T1, T2> self, Type type)
        {
            return self.MapTo(type);
        }

        public static object ToType<T1, T2, T>(this IDictionary<T1, T2> self)
        {
            return self.MapTo<T>();
        }

        public static IDictionary<string, object> ToNameValueDictionary(this object self)
        {
            return self.ToNameValueDictionary<object>();
        }

        public static IDictionary<string, TValue> ToNameValueDictionary<TValue>(this object self)
        {
            var dictionary = self as IDictionary;
            if (dictionary != null)
            {
                var nameValueDictionary = new Dictionary<string, TValue>();
                foreach (var key in dictionary.Keys)
                {
                    var keyType = key.GetType();
                    var keyString = keyType.GetTypeInfo().IsEnum
                        ? key.ToType(Enum.GetUnderlyingType(keyType)).ToString()
                        : key.ToString();

                    var value = dictionary[key];
                    var isObjectValueType = typeof(TValue) == typeof(object);
                    var typedValue = isObjectValueType ? (TValue)value : value.ToType<TValue>();
                    nameValueDictionary.Add(keyString, typedValue);
                }

                return nameValueDictionary;
            }

            var json = self.ToJson(new JsonSerializerSettings { ContractResolver = new SuppressErrorsContractResolver() });
            return json.FromJson<IDictionary<string, TValue>>();
        }

        public static IDictionary<string, object> ToNameValueDictionaryWithDefaults(this object self, object defaults)
        {
            return ToNameValueDictionaryWithDefaults(self, defaults?.ToNameValueDictionary());
        }

        public static IDictionary<string, TValue> ToNameValueDictionaryWithDefaults<TValue>(this object self, object defaults)
        {
            return ToNameValueDictionaryWithDefaults<TValue>(self, defaults?.ToNameValueDictionary());
        }

        public static IDictionary<string, object> ToNameValueDictionaryWithDefaults(this object self, IDictionary<string, object> defaults)
        {
            return self.ToNameValueDictionaryWithDefaults<object>(defaults);
        }

        public static IDictionary<string, TValue> ToNameValueDictionaryWithDefaults<TValue>(this object self, IDictionary<string, TValue> defaults)
        {
            var selfNameValueDictionary = self?.ToNameValueDictionary<TValue>() ?? new Dictionary<string, TValue>();
            var defaultsNameValueDictionary = defaults ?? new Dictionary<string, TValue>();

            return defaultsNameValueDictionary.Union(selfNameValueDictionary).ToDictionary(d => d.Key, d => d.Value);
        }
    }

    public class SuppressErrorsContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = instance =>
            {
                try
                {
                    var prop = (PropertyInfo)member;
                    if (prop.CanRead)
                    {
                        prop.GetValue(instance, null);
                        return true;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                return false;
            };

            return property;
        }
    }
}