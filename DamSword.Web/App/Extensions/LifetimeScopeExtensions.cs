using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using DamSword.Common;

namespace DamSword.Web
{
    public static class LifetimeScopeExtensions
    {
        public static IEnumerable<Type> GetImplementingTypes(this ILifetimeScope self, Type type)
        {
            return self.ComponentRegistry
                .RegistrationsFor(new TypedService(type))
                .Select(r => r.Activator.LimitType)
                .ToArray();
        }

        public static IEnumerable<Type> GetImplementingTypes<T>(this ILifetimeScope self)
        {
            var type = typeof(T);
            return self.GetImplementingTypes(type);
        }

        public static IEnumerable<Type> GetGenericImplementingTypes(this ILifetimeScope self, Type type)
        {
            return self.ComponentRegistry.Registrations
                .Where(r => r.Activator.LimitType.ImplementsGenericType(type))
                .Select(r => r.Activator.LimitType)
                .ToArray();
        }

        public static Type GetRegisteredType(this ILifetimeScope self, Type type)
        {
            if (self.IsRegistered(type))
                return type;

            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (self.IsRegistered(interfaceType))
                    return interfaceType;
            }

            throw new InvalidOperationException($"Type \"{type.FullName}\" is not registered.");
        }
    }
}