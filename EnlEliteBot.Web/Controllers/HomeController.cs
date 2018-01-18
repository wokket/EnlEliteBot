using Microsoft.AspNetCore.Mvc;

namespace EnlEliteBot.Web.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return StatusCode(200);
        }
    }
}