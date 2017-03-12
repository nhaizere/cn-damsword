using System;

namespace DamSword.Common
{
    public interface IScope : IDisposable
    {
    }

    public abstract class ScopeBase<TScope> : IScope
        where TScope : class, IScope
    {
        public static TScope Current => ScopeContainer.Current.Peek<TScope>();

        protected ScopeBase()
        {
            ScopeContainer.Current.Push<TScope>(this);
        }

        protected virtual void CloseScope()
        {
        }

        public void Dispose()
        {
            var disposedScope = ScopeContainer.Current.Pop<TScope>();
            if (disposedScope != this)
                throw new InvalidOperationException($"Unexpected scope of type \"{GetType().FullName}\" was removed from stack. Did you forget to dispose last scope?");

            CloseScope();
        }
    }
}
