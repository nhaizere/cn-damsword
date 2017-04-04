using System.Linq;
using System.Net;
using DamSword.Common;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services;
using DamSword.Web.Attributes;
using DamSword.Web.DTO;
using DamSword.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers.Api
{
    public class UserController : RestControllerBase<User, DTO.Objects.User>
    {
        public UserController(IEntityRepository<User> repository, IEntityService<User> service, IUnitOfWork unitOfWork)
            : base(repository, service, unitOfWork)
        {
        }

        [Require(UserPermissions.ViewUsers)]
        public override IActionResult List(Request<GenericListFetch> request)
        {
            var items = Repository
                .QueryApiListRequest(request.Data)
                .Where(u => u.Id == CurrentUser.Id || u.HierarchyLevel >= CurrentUser.Hierarchy)
                .Select(u => new DTO.Objects.User
                {
                    Id = u.Id,
                    Alias = u.Alias,
                    Login = u.Login,
                    HierarchyLevel = CurrentPermissions.Has(UserPermissions.ManageUsers) ? (int?)u.HierarchyLevel : null,
                    Permissions = CurrentPermissions.Has(UserPermissions.ManageUsers) ? (UserPermissions?)u.Permissions : null,
                    PersonId = CurrentPermissions.Has(UserPermissions.ManageUsers) ? u.PersonId : null
                })
                .ToArray();

            return this.ApiListResult(request, items);
        }

        [Require(UserPermissions.ViewUsers)]
        public override IActionResult Details(Request<long> request)
        {
            var item = Repository
                .SingleOrDefault(u => u.Id == request.Data && (u.Id == CurrentUser.Id || u.HierarchyLevel >= CurrentUser.Hierarchy), u => new DTO.Objects.User
                {
                    Id = u.Id,
                    Alias = u.Alias,
                    Login = CurrentPermissions.Has(UserPermissions.ManageUsers) ? u.Login : null,
                    HierarchyLevel = CurrentPermissions.Has(UserPermissions.ManageUsers) ? (int?)u.HierarchyLevel : null,
                    Permissions = CurrentPermissions.Has(UserPermissions.ManageUsers) ? (UserPermissions?)u.Permissions : null,
                    PersonId = u.PersonId
                });

            if (item == null)
                throw new RequestException(HttpStatusCode.NotFound);

            return this.ApiResult(request, item);
        }

        [Require(UserPermissions.ManageUsers)]
        public override IActionResult Create(Request<DTO.Objects.User> request)
        {
            var id = Store(request.Data, o => new User
            {
                Alias = o.Alias,
                Login = o.Login,
                PasswordHash = o.Password != null ? PasswordUtils.CreateHash(o.Password) : null,
                HierarchyLevel = o.HierarchyLevel ?? int.MaxValue,
                Permissions = o.Permissions ?? UserPermissions.Default
            });
            
            return this.ApiResult(request, id);
        }

        [Require(UserPermissions.ManageUsers)]
        public override IActionResult Update(Request<DTO.Objects.User> request)
        {
            var id = Store(request.Data, o => new User
            {
                Alias = o.Alias,
                Login = o.Login,
                HierarchyLevel = o.HierarchyLevel ?? int.MaxValue,
                Permissions = o.Permissions ?? UserPermissions.Default,
                PersonId = o.PersonId
            });

            return this.ApiSuccessResult(request);
        }
    }
}