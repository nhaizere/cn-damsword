using System.Linq;
using System.Reflection;

namespace DamSword.Common
{
    public static class MethodInfoExtensions
    {
        public static bool IsLinqToObjectsMethod(this MemberInfo self)
        {
            return self.DeclaringType == typeof(Enumerable);
        }
    }
}