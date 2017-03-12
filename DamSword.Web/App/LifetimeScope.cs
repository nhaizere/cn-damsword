using System;
using Autofac;
using DamSword.Common;

namespace DamSword.Web
{
    public class LifetimeScope : ScopeBase<LifetimeScope>
    {
        public ILifetimeScope AutofacLifetimeScope { get; }
        private readonly bool _disposeLifetimeScope;

        public LifetimeScope()
        {
            var lifetimeScope = ServiceLocator.Resolve<ILifetimeScope>();
            AutofacLifetimeScope = lifetimeScope.BeginLifetimeScope();
            _disposeLifetimeScope = true;
        }

        public LifetimeScope(ILifetimeScope lifetimeScope, bool disposeLifetimeScope = false)
        {
            if (lifetimeScope == null)
                throw new ArgumentNullException(nameof(lifetimeScope));

            AutofacLifetimeScope = lifetimeScope;
            _disposeLifetimeScope = disposeLifetimeScope;
        }

        protected override void CloseScope()
        {
            if (_disposeLifetimeScope)
                AutofacLifetimeScope.Dispose();
        }
    }
}