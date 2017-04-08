using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Services.Entity
{
    public interface IMetaAccountService : IEntityService<MetaAccount>
    {
    }

    public class MetaAccountService : IMetaAccountService
    {
        public IMetaAccountRepository MetaAccountRepository { get; set; }
        public IWebResourceAccountIdValidator WebResourceAccountIdValidator { get; set; }

        public void Save(MetaAccount entity)
        {
            if (!WebResourceAccountIdValidator.IsValidAccountId(entity.WebResourceId, entity.AccountId))
                throw this.ValidationException(e => e.AccountId, "Invalid.");

            MetaAccountRepository.Save(entity);
        }

        public void Delete(long id)
        {
            MetaAccountRepository.Delete(id);
        }
    }
}