using DamSword.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return new EmptyResult();
        }
    }
}