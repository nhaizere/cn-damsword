using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Details(int id)
        {
            var error = $"{id}";
            return View("~/Views/Error/Details.cshtml", error);
        }
    }
}