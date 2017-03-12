using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DamSword.Common
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type self)
        {
            return self.GetTypeInfo().IsValueType ? Activator.CreateInstance(self) : null;
        }

        public static bool IsDefaultValue(this Type self, object value)
        {
            return self.GetDefaultValue()?.Equals(value) == true;
        }

        public static bool IsNullable(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsSubclassOfGeneric(this Type self, Type genericType)
        {
            while (self != null)
            {
                var current = self.GetTypeInfo().IsGenericType ? self.GetGenericTypeDefinition() : self;
                if (genericType == current)
                    return true;

                self = self.GetTypeInfo().BaseType;
            }

            return false;
        }

        public static bool ImplementsInterface(this Type self, Type type)
        {
            return self.GetInterfaces().Any(i => i == type);
        }

        public static bool ImplementsInterface<TInterface>(this Type self)
        {
            return self.ImplementsInterface(typeof(TInterface));
        }

        public static bool ImplementsGenericType(this Type self, Type genericType)
        {
            Func<Type, bool> isAssignableFromGenericTypeFunc = t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == genericType;
            if (isAssignableFromGenericTypeFunc(self))
                return true;

            var interfaceTypes = self.GetInterfaces();
            if (interfaceTypes.Any(isAssignableFromGenericTypeFunc))
                return true;

            return self.GetTypeInfo().BaseType != null && self.GetTypeInfo().BaseType.ImplementsGenericType(genericType);
        }

        public static bool IsPrimitiveType(this Type self)
        {
            return self.GetTypeInfo().IsPrimitive || self.GetTypeInfo().IsValueType || self == typeof(string);
        }

        public static bool IsEnumerable(this Type self)
        {
            return typeof(IEnumerable).IsAssignableFrom(self);
        }

        public static bool IsComplexType(this Type self)
        {
            return !self.IsPrimitiveType();
        }

        public static bool IsExtensionMethod(this MethodInfo self)
        {
            return self.IsDefined(typeof(ExtensionAttribute), true);
        }

        public static bool IsStatic(this MemberInfo self)
        {
            switch (self.MemberType)
            {
                case MemberTypes.Constructor:
                    return ((ConstructorInfo)self).IsStatic;
                case MemberTypes.Event:
                    return ((EventInfo)self).GetAddMethod().IsStatic;
                case MemberTypes.Field:
                    return ((FieldInfo)self).IsStatic;
                case MemberTypes.Method:
                    return ((MethodInfo)self).IsStatic;
                case MemberTypes.Property:
                    {
                        var propertyInfo = (PropertyInfo)self;
                        return (propertyInfo.GetMethod ?? propertyInfo.SetMethod).IsStatic;
                    }
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                case MemberTypes.Custom:
                case MemberTypes.All:
                    throw new NotSupportedException($"Unable to resolve is Member static for member of type \"{self.MemberType}\".");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IEnumerable<Attribute> GetAttributes(this ICustomAttributeProvider self, Type attributeType, bool inherit = false)
        {
            return GetMatchedAttributes(self, attributeType, inherit);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider self, bool inherit = false)
        {
            var attributeType = typeof(TAttribute);
            return self.GetAttributes(attributeType, inherit).Cast<TAttribute>().ToArray();
        }

        public static Attribute GetAttribute(this ICustomAttributeProvider self, Type attributeType, bool inherit = false)
        {
            return self.GetAttributes(attributeType, inherit).FirstOrDefault();
        }

        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider self, bool inherit = false)
            where TAttribute : Attribute
        {
            return self.GetAttributes<TAttribute>(inherit).FirstOrDefault();
        }

        public static bool HasAttribute(this ICustomAttributeProvider self, Type attributeType, bool inherit = false)
        {
            var matchedAttributes = GetMatchedAttributes(self, attributeType, inherit);
            return matchedAttributes.Any();
        }

        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider self, bool inherit = false)
        {
            var attributeType = typeof(TAttribute);
            return self.HasAttribute(attributeType, inherit);
        }

        public static IEnumerable<MethodInfo> GetBestMatchedMethods(this Type self, string name, IEnumerable<object> parameters, BindingFlags? bindingFlags = null)
        {
            var byName = (bindingFlags.HasValue ? self.GetMethods(bindingFlags.Value) : self.GetMethods()).Where(m => m.Name == name);
            var byParameters = byName.Where(m => m.GetParameters().Length >= parameters.Count() &&
                parameters.Select((p, i) => m.GetParameters()[i].ParameterType == typeof(object) || m.GetParameters()[i].ParameterType.IsInstanceOfType(p)).All(r => r));

            return byParameters.ToArray();
        }

        private static Attribute[] GetMatchedAttributes(ICustomAttributeProvider attributeProvider, Type type, bool inherit)
        {
            if (attributeProvider == null)
                throw new ArgumentNullException(nameof(attributeProvider));
            if (type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsSubclassOf(typeof(Attribute)))
                throw new ArgumentException($"Must implement interface {typeof(Attribute).FullName}.", nameof(type));

            return attributeProvider.GetCustomAttributes(inherit)
                .Where(a =>
                {
                    var attributeType = a.GetType();
                    return (type.GetTypeInfo().IsClass && (attributeType == type || attributeType.GetTypeInfo().IsSubclassOf(type))) ||
                        (type.GetTypeInfo().IsInterface && attributeType.ImplementsInterface(type));
                })
                .Cast<Attribute>()
                .ToArray();
        }
    }
}
