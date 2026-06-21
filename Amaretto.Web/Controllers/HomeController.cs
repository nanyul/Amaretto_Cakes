using Amaretto.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            //ViewBag.Id = id;
            return View();

        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        //[HttpGet]
        //public IActionResult ErrorHandler(string messagesJson)
        //{
        //    var errorMessages = JsonConvert.
        //        DeserializeObject<ErrorMiddlewareViewModel>(messagesJson);
        //    ViewBag.ErrorMessages = errorMessages;
        //    return View("ErrorHandler");
        //}
    }
}
