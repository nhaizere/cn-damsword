using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Services.Entity
{
    public interface IPersonService : IEntityService<Person>
    {
    }

    public class PersonService : IPersonService
    {
        public IPersonRepository PersonRepository { get; set; }

        public void Save(Person entity)
        {
            PersonRepository.Save(entity);
        }

        public void Delete(long id)
        {
            PersonRepository.Delete(id);
        }
    }
}