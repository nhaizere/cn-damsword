using DamSword.Data.Entities;

namespace DamSword.Data.Repositories
{
    public interface IPersonRepository : IEntityRepository<Person>
    {
    }

    public class PersonRepository : EntityRepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(IEntityContext entityContext, bool asNoTracking)
            : base(entityContext, asNoTracking)
        {
        }
    }
}