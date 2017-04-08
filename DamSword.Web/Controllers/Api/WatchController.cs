using System;
using System.Linq.Expressions;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services;
using DamSword.Services.Entity;
using DamSword.Web.Attributes;
using DamSword.Web.DTO;
using DamSword.Web.DTO.Watch;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers.Api
{
    public class WatchController : RestControllerBase<Data.Entities.Watch, DTO.Objects.Watch>
    {
        public WatchController(IEntityRepository<Data.Entities.Watch> repository, IEntityService<Data.Entities.Watch> service, IUnitOfWork unitOfWork)
            : base(repository, service, unitOfWork)
        {
        }

        protected override UserPermissions? CreatePermissions => UserPermissions.Owner;
        protected override UserPermissions? ReadPermissions => UserPermissions.Owner;
        protected override UserPermissions? UpdatePermissions => UserPermissions.Owner;
        protected override UserPermissions? DeletePermissions => UserPermissions.Owner;

        protected override Expression<Func<Data.Entities.Watch, DTO.Objects.Watch>> ReadMap =>
            e => new DTO.Objects.Watch
            {
                Id = e.Id,
                PersonId = e.PersonId,
                WebResourceId = e.WebResourceId
            };

        protected override Expression<Func<DTO.Objects.Watch, Data.Entities.Watch>> WriteMap =>
            o => new Data.Entities.Watch
            {
                Id = o.Id,
                PersonId = o.PersonId,
                WebResourceId = o.WebResourceId
            };

        [HttpPost, AjaxOnly]
        public IActionResult Set([FromBody] Request<Set> request)
        {
            ((IWatchService)Service).Set(request.Data.WebResourceId, request.Data.AccountId);
            UnitOfWork.Commit();

            return this.ApiSuccessResult(request);
        }
    }
}