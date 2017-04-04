using DamSword.Common;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;

namespace DamSword.Services.Entity
{
    public interface IUserService : IEntityService<User>
    {
    }

    public class UserService : IUserService
    {
        public IUserRepository UserRepository { get; set; }
        public ICurrentUserProvider CurrentUserProvider { get; set; }

        public void Save(User entity)
        {
            if (entity.PasswordHash.IsNullOrEmpty())
                throw this.ValidationException(e => e.PasswordHash, $"Password must be set.");

            var hasOwner = UserRepository.GetOwner() != null;
            if (!hasOwner)
            {
                entity.HierarchyLevel = 0;
                entity.Permissions = UserPermissions.Owner;
            }
            else
            {
                if (entity.HierarchyLevel == 0)
                    throw this.ValidationException(e => e.HierarchyLevel, $"{nameof(entity.HierarchyLevel).SplitUpperCaseBySpace()} \"0\" is system reserved value.");

                var currentUser = CurrentUserProvider.GetCurrentUser();
                if (entity.HierarchyLevel <= (currentUser?.HierarchyLevel ?? int.MaxValue))
                    throw this.ValidationException(e => e.HierarchyLevel, $"Unable to set {nameof(entity.HierarchyLevel).SplitUpperCaseBySpace()} higher or equal to own.");
            }

            UserRepository.Save(entity);
        }

        public void Delete(long id)
        {
            UserRepository.Delete(id);
        }
    }
}