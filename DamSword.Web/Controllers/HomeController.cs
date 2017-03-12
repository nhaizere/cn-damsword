using DamSword.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;

        public HomeController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ActionResult Index()
        {
            return new EmptyResult();
        }
    }
}