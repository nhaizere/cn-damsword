using System;
using System.Collections.Generic;

namespace DamSword.Common
{
    public class ScopeContainer
    {
        private readonly IDictionary<Type, Stack<IScope>> _scopeStackDictionary = new Dictionary<Type, Stack<IScope>>();

        private static Func<ScopeContainer> _scopeStackResolver;

        public static ScopeContainer Current
        {
            get
            {
                if (_scopeStackResolver == null)
                    throw new InvalidOperationException($"\"{typeof(ScopeContainer).FullName}\" resolver need to be set.");

                return _scopeStackResolver();
            }
        }

        public static void SetScopeStackResolver(Func<ScopeContainer> scopeStackResolver)
        {
            _scopeStackResolver = scopeStackResolver;
        }

        public void Push<TScope>(IScope scope)
            where TScope : IScope
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            GetStackFor(typeof(TScope)).Push(scope);
        }

        public TScope Pop<TScope>()
            where TScope : IScope
        {
            return (TScope)GetStackFor(typeof(TScope)).Pop();
        }

        public TScope Peek<TScope>()
            where TScope : IScope
        {
            var stack = GetStackFor(typeof(TScope));
            return (TScope)(stack.Count > 0 ? stack.Peek() : null);
        }

        private Stack<IScope> GetStackFor(Type type)
        {
            return _scopeStackDictionary.ContainsKey(type)
                ? _scopeStackDictionary[type]
                : _scopeStackDictionary[type] = new Stack<IScope>();
        }
    }
}