using System;
using System.Collections.Generic;
using DamSword.Common;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Services.Entity
{
    public interface IWatchService : IEntityService<Watch>
    {
        void Set(long webResourceId, string accountId);
    }

    public class WatchService : IWatchService
    {
        public IWatchRepository WatchRepository { get; set; }
        public IMetaAccountRepository MetaAccountRepository { get; set; }
        public IMetaAccountService MetaAccountService { get; set; }
        public IPersonService PersonService { get; set; }
        public IWebResourceRepository WebResourceRepository { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IWebResourceAccountIdValidator WebResourceAccountIdValidator { get; set; }

        public void Save(Watch entity)
        {
            if (WatchRepository.Any(w => w.PersonId == entity.PersonId && w.WebResourceId == entity.WebResourceId))
                throw this.ValidationException(e => e.PersonId, $"Watch already set for this {typeof(Person).Name.SplitUpperCaseBySpace()} and {typeof(WebResource).Name.SplitUpperCaseBySpace()}.");

            WatchRepository.Save(entity);
        }

        public void Delete(long id)
        {
            WatchRepository.Delete(id);
        }

        public void Set(long webResourceId, string accountId)
        {
            if (!WebResourceRepository.Any(r => r.Id == webResourceId))
                throw new ArgumentException("Not exist.", nameof(webResourceId));

            var validAccountId = WebResourceAccountIdValidator.GetValidAccountId(webResourceId, accountId);
            if (validAccountId == null)
                throw new ArgumentException("Invalid.", nameof(accountId));

            var personId = MetaAccountRepository.FirstOrDefault(
                a => a.AccountId == validAccountId && a.WebResourceId == webResourceId,
                a => a.PersonId);

            if (personId == 0)
            {
                var person = new Person();
                PersonService.Save(person);
                UnitOfWork.Commit();

                personId = person.Id;
                var account = new MetaAccount
                {
                    PersonId = personId,
                    Type = MetaType.Created,
                    AccountId = validAccountId,
                    WebResourceId = webResourceId
                };

                MetaAccountService.Save(account);
                UnitOfWork.Commit();
            }

            if (WatchRepository.Any(w => w.PersonId == personId && w.WebResourceId == webResourceId))
                return;

            Save(new Watch
            {
                PersonId = personId,
                WebResourceId = webResourceId
            });
        }
    }
}