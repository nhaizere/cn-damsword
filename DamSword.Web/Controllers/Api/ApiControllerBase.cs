using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    public abstract class ApiControllerBase : Controller
    {
    }
}