using ManejadorDePresupuestos.Models;
using ManejadorDePresupuestos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejadorDePresupuestos.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;

        public CuentasController(IRepositorioTipoCuentas repositorioTipoCuentas, IServicioUsuarios servicioUsuarios,IRepositorioCuentas repositorioCuentas)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
        }

        [HttpGet]
        public async Task< IActionResult> Crear()
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioID);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult>Crear(CuentaCreacionViewModel modelo)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorID(modelo.ID, usuarioID);

            if(tipoCuenta == null) { return RedirectToAction("NoEncontrado", "Home"); }

            if(!ModelState.IsValid) {

                modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioID);
                return View(modelo);
            }

            await repositorioCuentas.Crear(modelo);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioID)
        {
            var tipoCuentas = await repositorioTipoCuentas.Obtener(usuarioID);
            return tipoCuentas.Select(x => new SelectListItem(x.Nombre, x.ID.ToString()));
        } 
    }
}
