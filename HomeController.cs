using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContactManagerStarter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ILogger<HomeController> homeControllerLogger = ControllerLogging.CreateLogger<HomeController>();
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                homeControllerLogger.LogWarning("Home Controller Warning" + ex.Message);
                return View();
            }
            
        }
    }
}
