using System;
using System.Linq.Expressions;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services;

namespace DamSword.Web.Controllers.Api
{
    public class WebResouceController : RestControllerBase<WebResource, DTO.Objects.WebResource>
    {
        public WebResouceController(IEntityRepository<WebResource> repository, IEntityService<WebResource> service, IUnitOfWork unitOfWork)
            : base(repository, service, unitOfWork)
        {
        }

        protected override UserPermissions? CreatePermissions => UserPermissions.Owner;
        protected override UserPermissions? ReadPermissions => UserPermissions.ViewWatches;
        protected override UserPermissions? UpdatePermissions => UserPermissions.Owner;
        protected override UserPermissions? DeletePermissions => UserPermissions.Owner;

        protected override Expression<Func<WebResource, DTO.Objects.WebResource>> ReadMap =>
            e => new DTO.Objects.WebResource
            {
                Id = e.Id,
                Name = e.Name,
                Url = e.Url,
                ImageUrl = e.ImageUrl
            };

        protected override Expression<Func<DTO.Objects.WebResource, WebResource>> WriteMap =>
            o => new WebResource
            {
                Id = o.Id,
                Name = o.Name,
                Url = o.Url,
                ImageUrl = o.ImageUrl
            };
    }
}