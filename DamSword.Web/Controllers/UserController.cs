using System.Linq;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Web.Attributes;
using DamSword.Web.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet, Authorize(Require = UserPermissions.ViewUsers)]
        public IActionResult List(string search)
        {
            var canManage = Permissions.Has(UserPermissions.ManageUsers);
            search = search?.ToLower() ?? string.Empty;

            var users = _userRepository.Select(u => u.Id == CurrentUser.Id || u.HierarchyLevel > CurrentUser.Hierarchy,
                u => new ListModel.User
                {
                    Id = u.Id,
                    Alias = u.Alias,
                    Login = canManage ? u.Login : null,
                    HierarchyLevel = canManage ? u.HierarchyLevel : int.MaxValue,
                    Permissions = canManage ? u.Permissions : UserPermissions.None
                })
                .Where(u => 
                    u.Id.ToString().ToLower().Contains(search) ||
                    u.Alias.ToLower().Contains(search) ||
                    canManage && (u.Login.ToLower().Contains(search) || u.HierarchyLevel.ToString().ToLower().Contains(search) || u.Permissions.ToString().ToLower().Contains(search)))
                .OrderBy(u => u.Alias)
                .ThenBy(u => u.HierarchyLevel)
                .ToArray();

            return View("~/Views/User/List.cshtml", new ListModel
            {
                Search = search,
                Users = users
            });
        }
    }
}