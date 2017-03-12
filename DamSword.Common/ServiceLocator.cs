using System;

namespace DamSword.Common
{
    public class ServiceLocator
    {
        private static Func<Type, object> _resolverFunc;

        public static void SetResolver(Func<Type, object> resolverFunc)
        {
            _resolverFunc = resolverFunc;
        }

        public static object Resolve(Type type)
        {
            return _resolverFunc(type);
        }

        public static T Resolve<T>()
        {
            var type = typeof(T);
            return (T)_resolverFunc(type);
        }
    }
}