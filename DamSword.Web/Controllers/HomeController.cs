﻿using System.Linq;
using DamSword.Common;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Watch;
using DamSword.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View("~/Views/Home/Dashboard.cshtml");
        }

        [HttpPost]
        public IActionResult Dashboard(string test)
        {
            Person person;
            if (ServiceLocator.Resolve<IPersonRepository>().Count() == 0)
            {
                person = new Person();
                
                var webResourceId = ServiceLocator.Resolve<IWebResourceRepository>()
                    .First(r => r.Name == "VKontakte", r => r.Id);

                ServiceLocator.Resolve<IPersonRepository>().Save(person);
                ServiceLocator.Resolve<IUnitOfWork>().Commit();

                ServiceLocator.Resolve<IMetaNameRepository>().Save(new MetaName
                {
                    PersonId = person.Id,
                    NameType = MetaNameType.Real,
                    NameKind = MetaNameKind.Alias,
                    Name = "TheQueen"
                });

                ServiceLocator.Resolve<IMetaAccountRepository>().Save(new MetaAccount
                {
                    PersonId = person.Id,
                    AccountId = "id158474048",
                    WebResourceId = webResourceId
                });

                ServiceLocator.Resolve<IUnitOfWork>().Commit();
            }
            else
            {
                person = ServiceLocator.Resolve<IPersonRepository>().Single(p => p.Names.Any(n => n.NameKind == MetaNameKind.Alias && n.Name == "TheQueen"));
            }

            ServiceLocator.Resolve<IWatchService>().FetchOnline(new[] { person.Id });
            return View("~/Views/Home/Dashboard.cshtml");
        }
    }
}