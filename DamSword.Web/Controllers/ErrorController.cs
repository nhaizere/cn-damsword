using DamSword.Web.Models.Error;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Details(int id)
        {
            return View("~/Views/Error/Details.cshtml", new DetailsModel
            {
                StatusCode = id,
                Exception = null
            });
        }
    }
}