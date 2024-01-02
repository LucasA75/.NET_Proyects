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
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorID(modelo.TipoCuentaID,usuarioID);

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

        public async Task<IActionResult> Index()
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var cuentasConTipoCuenta = await repositorioCuentas.Buscar(usuarioID);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var cuentaAEditar = await repositorioCuentas.ObtenerCuentaPorID(id,usuarioID);

            if(cuentaAEditar == null) { return RedirectToAction("NoEncontrado","Home"); }

            var modelo = new CuentaCreacionViewModel()
            {
                ID = cuentaAEditar.ID,
                Nombre = cuentaAEditar.Nombre,
                Descripcion = cuentaAEditar.Descripcion,
                Balance = cuentaAEditar.Balance,
                TipoCuentaID = cuentaAEditar.TipoCuentaID
            };

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioID);

            return View(modelo);
        }
    }
}
