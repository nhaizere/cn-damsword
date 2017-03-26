using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Views/Shared/Components/Navigation/Navigation.cshtml");
        }
    }
}