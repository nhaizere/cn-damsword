using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Web.DatabaseInitializers
{
    public class TestDatabaseInitializer : IDatabaseInitializer
    {
        public IUserRepository UserRepository { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }

        public void Initialize(IEntityContext context)
        {
            UserRepository.Save(new User
            {
                Alias = "Administrator",
                Login = "admin",
                PasswordHash = PasswordUtils.CreateHash("123")
            });

            UnitOfWork.Commit();
        }
    }
}