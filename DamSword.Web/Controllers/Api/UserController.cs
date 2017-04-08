using System;
using System.Linq.Expressions;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services;

namespace DamSword.Web.Controllers.Api
{
    public class UserController : RestControllerBase<User, DTO.Objects.User>
    {
        public UserController(IEntityRepository<User> repository, IEntityService<User> service, IUnitOfWork unitOfWork)
            : base(repository, service, unitOfWork)
        {
        }

        protected override UserPermissions? CreatePermissions => UserPermissions.ManageUsers;
        protected override UserPermissions? ReadPermissions => UserPermissions.ViewUsers;
        protected override UserPermissions? UpdatePermissions => UserPermissions.ManageUsers;
        protected override UserPermissions? DeletePermissions => UserPermissions.ManageUsers;

        protected override Expression<Func<User, bool>> Predicate =>
            e => e.Id == CurrentUser.Id || e.HierarchyLevel >= CurrentUser.Hierarchy;

        protected override Expression<Func<User, DTO.Objects.User>> ReadMap =>
            e => new DTO.Objects.User
            {
                Id = e.Id,
                Alias = e.Alias,
                Login = e.Login,
                HierarchyLevel = CurrentPermissions.Has(UserPermissions.ManageUsers) ? (int?)e.HierarchyLevel : null,
                Permissions = CurrentPermissions.Has(UserPermissions.ManageUsers) ? (UserPermissions?)e.Permissions : null,
                PersonId = CurrentPermissions.Has(UserPermissions.ManageUsers) ? e.PersonId : null
            };

        protected override Expression<Func<DTO.Objects.User, User>> WriteMap =>
            o => new User
            {
                Alias = o.Alias,
                Login = o.Login,
                PasswordHash = o.Password != null ? PasswordUtils.CreateHash(o.Password) : null,
                HierarchyLevel = o.HierarchyLevel ?? int.MaxValue,
                Permissions = o.Permissions ?? UserPermissions.Default,
                PersonId = o.PersonId
            };
    }
}