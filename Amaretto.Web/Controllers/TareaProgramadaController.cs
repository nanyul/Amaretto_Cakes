using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class TareaProgramadaController : Controller
    {
        private readonly ITareaProgramadaEstado _estado;

        public TareaProgramadaController(ITareaProgramadaEstado estado)
        {
            _estado = estado;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_estado);
        }
    }
}
