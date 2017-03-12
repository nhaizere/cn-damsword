using DamSword.Data;

namespace DamSword.Web.DatabaseInitializers
{
    public interface IDatabaseInitializer
    {
        void Initialize(IEntityContext context);
    }
}