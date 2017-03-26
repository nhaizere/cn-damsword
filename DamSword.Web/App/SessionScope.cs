using DamSword.Common;
using DamSword.Data.Entities;

namespace DamSword.Web
{
    public class SessionScope : ScopeBase<SessionScope>
    {
        public Session Session { get; }

        public SessionScope(Session session)
        {
            Session = session;
        }
    }
}