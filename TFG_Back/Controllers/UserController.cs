using Microsoft.AspNetCore.Mvc;

namespace TFG_Back.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
