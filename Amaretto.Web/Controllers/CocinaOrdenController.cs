using Amaretto.Application.Services.Implementations;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Repository.Interfaces;
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

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return RedirectToAction("Index");
                }

                var pasos = await _serviceCocinaOrden.ListByDetalleAsync(id);

                if (pasos == null || !pasos.Any())
                {
                    return RedirectToAction("Index");
                }

                return View(pasos);
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

            if (estacionIds != null && ordenPasos != null && estacionIds.Length != ordenPasos.Length)
            {
                ModelState.AddModelError("", "La cantidad de órdenes no coincide con la cantidad de estaciones seleccionadas");
            }
            else if (estacionIds != null && ordenPasos != null && estacionIds.Length > 0)
            {
                var cantidad = estacionIds.Length;

                // No se pueden repetir números de orden
                if (ordenPasos.Distinct().Count() != ordenPasos.Length)
                    ModelState.AddModelError("", "Las estaciones no pueden tener el mismo número de orden");

                // El rango debe ir de 1 a la cantidad de estaciones seleccionadas
                if (ordenPasos.Any(o => o < 1 || o > cantidad))
                    ModelState.AddModelError("", $"El número de orden debe estar entre 1 y {cantidad} (la cantidad de estaciones seleccionadas)");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ListPedidoDetalle = await _servicePedidoDetalle.ListDisponiblesAsync();
                ViewBag.ListEstaciones = await _serviceEstacion.ListAsync();

                TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                    "Crear Proceso",
                    string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)),
                    Util.SweetAlertMessageType.error);

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

        // EDITAR: cambiar orden y estado de cada estación
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pasos = await _serviceCocinaOrden.ListByDetalleAsync(id);
            if (pasos == null || !pasos.Any())
                return RedirectToAction("Index");

            var detalle = await _servicePedidoDetalle.FindByIdAsync(id);
            if (detalle == null)
                return RedirectToAction("Index");

            var todasLasEstaciones = await _serviceEstacion.ListAsync();
            var idsUsados = pasos.Select(p => p.IdEstacion).ToHashSet();

            ViewBag.Detalle = detalle;
            ViewBag.Pasos = pasos.OrderBy(p => p.OrdenPaso).ToList();
            ViewBag.ListEstacionesDisponibles = todasLasEstaciones
                .Where(e => !idsUsados.Contains(e.IdEstacion))
                .ToList();

            return View((object)id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int idDetalle,
            int[] cocinaOrdenIds, int[] ordenPasos, string[] estados,
            int[] nuevasEstacionIds, int[] nuevosOrdenPasos)
        {
            cocinaOrdenIds ??= Array.Empty<int>();
            ordenPasos ??= Array.Empty<int>();
            estados ??= Array.Empty<string>();
            nuevasEstacionIds ??= Array.Empty<int>();
            nuevosOrdenPasos ??= Array.Empty<int>();

            if (cocinaOrdenIds.Length != estados.Length || cocinaOrdenIds.Length != ordenPasos.Length)
                ModelState.AddModelError("", "Datos de estaciones existentes inválidos");

            if (nuevasEstacionIds.Length != nuevosOrdenPasos.Length)
                ModelState.AddModelError("", "Datos de estaciones nuevas inválidos");

            if (ModelState.IsValid)
            {
                // El orden se valida en conjunto: las existentes + las nuevas que se agregan
                var todosLosOrdenes = ordenPasos.Concat(nuevosOrdenPasos).ToList();
                var cantidad = todosLosOrdenes.Count;

                if (todosLosOrdenes.Distinct().Count() != todosLosOrdenes.Count)
                    ModelState.AddModelError("", "Las estaciones no pueden tener el mismo número de orden");

                if (todosLosOrdenes.Any(o => o < 1 || o > cantidad))
                    ModelState.AddModelError("", $"El número de orden debe estar entre 1 y {cantidad}");
            }

            if (!ModelState.IsValid)
            {
                var pasosInvalidos = await _serviceCocinaOrden.ListByDetalleAsync(idDetalle);
                var todasLasEstaciones = await _serviceEstacion.ListAsync();
                var idsUsados = pasosInvalidos.Select(p => p.IdEstacion).ToHashSet();

                ViewBag.Detalle = await _servicePedidoDetalle.FindByIdAsync(idDetalle);
                ViewBag.Pasos = pasosInvalidos.OrderBy(p => p.OrdenPaso).ToList();
                ViewBag.ListEstacionesDisponibles = todasLasEstaciones
                    .Where(e => !idsUsados.Contains(e.IdEstacion))
                    .ToList();

                TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                    "Editar Proceso",
                    string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)),
                    Util.SweetAlertMessageType.error);

                return View(idDetalle);
            }

            var filasExistentes = cocinaOrdenIds
                .Select((id, i) => new CocinaOrdenUpdateInput(id, ordenPasos[i], estados[i]))
                .ToList();

            var filasNuevas = nuevasEstacionIds
                .Select((id, i) => new CocinaOrdenEstacionInput(id, nuevosOrdenPasos[i]))
                .ToList();

            try
            {
                await _serviceCocinaOrden.UpdateEstadoAsync(idDetalle, filasExistentes, filasNuevas);
            }
            catch (InvalidOperationException ex)
            {
                var pasosActuales = await _serviceCocinaOrden.ListByDetalleAsync(idDetalle);
                var todasLasEstaciones = await _serviceEstacion.ListAsync();
                var idsUsados = pasosActuales.Select(p => p.IdEstacion).ToHashSet();

                ViewBag.Detalle = await _servicePedidoDetalle.FindByIdAsync(idDetalle);
                ViewBag.Pasos = pasosActuales.OrderBy(p => p.OrdenPaso).ToList();
                ViewBag.ListEstacionesDisponibles = todasLasEstaciones
                    .Where(e => !idsUsados.Contains(e.IdEstacion))
                    .ToList();

                TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                    "Editar Proceso", ex.Message, Util.SweetAlertMessageType.error);

                return View(idDetalle);
            }

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Editar Proceso",
                "Progreso del proceso actualizado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }
    }
}