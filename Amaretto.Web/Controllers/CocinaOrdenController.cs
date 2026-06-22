using Amaretto.Application.Services.Implementations;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class CocinaOrdenController : Controller
    {
        private readonly IServiceCocinaOrden _serviceCocinaOrden;
        private readonly IServiceEstacion _serviceEstacion;

        public CocinaOrdenController(IServiceCocinaOrden serviceCocinaOrden, IServiceEstacion serviceEstacion)
        {
            _serviceCocinaOrden = serviceCocinaOrden;
            _serviceEstacion = serviceEstacion;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var collection = await _serviceCocinaOrden.ListAsync();

            return View(collection);
        }

        public async Task<ActionResult> Details(string? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                var @object = await _serviceCocinaOrden.FindByIdAsync(id);

                if (@object == null)
                {
                    throw new Exception("Libro no existente");

                }

                ViewBag.Estaciones = await _serviceEstacion.ListAsync();

                return View(@object);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
