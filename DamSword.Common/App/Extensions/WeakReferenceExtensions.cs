using System;

namespace DamSword.Common
{
    public static class WeakReferenceExtensions
    {
        public static TTarget GetOrFetchTarget<TTarget>(this WeakReference<TTarget> self, Func<TTarget> factoryMethod)
            where TTarget : class
        {
            TTarget target;
            if (self.TryGetTarget(out target))
                return target;

            target = factoryMethod();
            self.SetTarget(target);

            return target;
        }
    }
}
