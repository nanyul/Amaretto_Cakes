using Amaretto.Application.Services.Implementations;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Amaretto.Web.Controllers
{
    public class CocinaOrdenController : Controller
    {
        private readonly IServiceCocinaOrden _serviceCocinaOrden;
        private readonly IServiceEstacion _serviceEstacion;
        private readonly IServicePedidoDetalle _servicePedidoDetalle;

        public CocinaOrdenController(
            IServiceCocinaOrden serviceCocinaOrden,
            IServiceEstacion serviceEstacion,
            IServicePedidoDetalle servicePedidoDetalle)
        {
            _serviceCocinaOrden = serviceCocinaOrden;
            _serviceEstacion = serviceEstacion;
            _servicePedidoDetalle = servicePedidoDetalle;
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

        // CREAR
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.ListPedidoDetalle = await _servicePedidoDetalle.ListDisponiblesAsync();
            ViewBag.ListEstaciones = await _serviceEstacion.ListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int idDetalle, int[] estacionIds, int[] ordenPasos)
        {
            if (idDetalle <= 0)
                ModelState.AddModelError("idDetalle", "Debe seleccionar un producto/pedido");

            if (estacionIds == null || estacionIds.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos una estación");

            if (!ModelState.IsValid)
            {
                ViewBag.ListPedidoDetalle = await _servicePedidoDetalle.ListDisponiblesAsync();
                ViewBag.ListEstaciones = await _serviceEstacion.ListAsync();
                return View();
            }

            var estaciones = estacionIds
                .Select((id, i) => (IdEstacion: id, OrdenPaso: ordenPasos[i]))
                .ToList();

            await _serviceCocinaOrden.AddAsync(idDetalle, estaciones);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Crear Proceso",
                "Proceso de preparación creado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }

        // EDITAR: avanzar el estado de cada estación (no cambia estaciones ni orden)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pasos = await _serviceCocinaOrden.ListByDetalleAsync(id);
            if (pasos == null || !pasos.Any())
                return RedirectToAction("Index");

            var detalle = await _servicePedidoDetalle.FindByIdAsync(id);
            if (detalle == null)
                return RedirectToAction("Index");

            ViewBag.Detalle = detalle;
            ViewBag.Pasos = pasos.OrderBy(p => p.OrdenPaso).ToList();

            return View((object)id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idDetalle, int[] cocinaOrdenIds, string[] estados)
        {
            if (cocinaOrdenIds == null || estados == null || cocinaOrdenIds.Length != estados.Length)
            {
                ModelState.AddModelError("", "Datos de estaciones inválidos");
            }

            if (!ModelState.IsValid)
            {
                var pasos = await _serviceCocinaOrden.ListByDetalleAsync(idDetalle);
                ViewBag.Detalle = await _servicePedidoDetalle.FindByIdAsync(idDetalle);
                ViewBag.Pasos = pasos.OrderBy(p => p.OrdenPaso).ToList();
                return View(idDetalle);
            }

            var estadosPorId = cocinaOrdenIds
                .Select((id, i) => (id, estado: estados[i]))
                .ToDictionary(x => x.id, x => x.estado);

            await _serviceCocinaOrden.UpdateEstadoAsync(idDetalle, estadosPorId);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Editar Proceso",
                "Progreso del proceso actualizado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }
    }
}
