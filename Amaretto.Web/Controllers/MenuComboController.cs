using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class MenuComboController : Controller
    {
        private readonly IServiceMenuCombo _serviceMenuCombo;
        private readonly IServiceCombo _serviceCombo;

        public MenuComboController(IServiceMenuCombo serviceMenuCombo, IServiceCombo serviceCombo)
        {
            _serviceMenuCombo = serviceMenuCombo;
            _serviceCombo = serviceCombo;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var menu = await _serviceMenuCombo.ListAsync();
            if (menu == null)
            {
                return NotFound("No existe un menú disponible actualmente.");
            }
            return View(menu);
        }

        [HttpGet]
        public async Task<ActionResult> Catalogo()
        {
            var menu = await _serviceMenuCombo.ListAsync();
            if (menu == null)
            {
                return NotFound("No existe un menú disponible actualmente.");
            }
            return View(menu);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                var @object = await _serviceMenuCombo.FindByIdAsync(id.Value);
                if (@object == null)
                {
                    throw new Exception("Menú no existente");
                }
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
            var combos = await _serviceCombo.ListAsync();
            ViewBag.ListCombos = combos.Where(c => c.Estado).ToList();
            return View(new MenuComboDTO { Estado = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuComboDTO dto, string[] selectedCombos)
        {
            ModelState.Remove("IdMenuCombo");

            if (selectedCombos == null || selectedCombos.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un combo");

            if (dto.FechaFin < dto.FechaInicio)
                ModelState.AddModelError("FechaFin", "La fecha fin debe ser posterior a la fecha inicio");

            if (!ModelState.IsValid)
            {
                var combos = await _serviceCombo.ListAsync();
                ViewBag.ListCombos = combos.Where(c => c.Estado).ToList();
                return View(dto);
            }

            await _serviceMenuCombo.AddAsync(dto, selectedCombos);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Crear Menú",
                "Menú de combos creado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }

        // EDITAR
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _serviceMenuCombo.FindByIdAsync(id);
            if (dto == null)
                return RedirectToAction("Index");

            var combos = await _serviceCombo.ListAsync();
            ViewBag.ListCombos = combos.Where(c => c.Estado).ToList();
            ViewBag.SelectedCombos = dto.MenuDetalleCombo.Select(x => x.IdCombo).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuComboDTO dto, string[] selectedCombos)
        {
            ModelState.Remove("IdMenuCombo");

            if (selectedCombos == null || selectedCombos.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un combo");

            if (dto.FechaFin < dto.FechaInicio)
                ModelState.AddModelError("FechaFin", "La fecha fin debe ser posterior a la fecha inicio");

            if (!ModelState.IsValid)
            {
                var combos = await _serviceCombo.ListAsync();
                ViewBag.ListCombos = combos.Where(c => c.Estado).ToList();
                ViewBag.SelectedCombos = selectedCombos?.ToList() ?? new List<string>();
                return View(dto);
            }

            await _serviceMenuCombo.UpdateAsync(id, dto, selectedCombos);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Editar Menú",
                "Menú de combos actualizado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }
    }
}
