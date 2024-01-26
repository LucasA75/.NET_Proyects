using AutoMapper;
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
        private readonly IMapper mapper;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReportes servicioReportes;

        public CuentasController(IRepositorioTipoCuentas repositorioTipoCuentas, IServicioUsuarios servicioUsuarios,IRepositorioCuentas repositorioCuentas,IMapper mapper, IRepositorioTransacciones repositorioTransacciones, IServicioReportes servicioReportes)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
        }

        [HttpGet]
        public async Task< IActionResult> Crear()
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioID);

            
            return View(modelo);
        }

        public async Task<IActionResult> Detalle(int id,int mes,int ano)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(id, usuarioID);

            if(cuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes.ObtenerTransaccionesDetalladasPorCuenta(usuarioID, id, mes, ano, ViewBag);

          
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

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuentaAEditar);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioID);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaAEditar)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(cuentaAEditar.ID, usuarioID);
            if(cuenta == null) { return RedirectToAction("NoEncontrado", "Home"); }

            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorID(cuenta.ID, usuarioID);
            if (tipoCuenta == null) { return RedirectToAction("NoEncontrado", "Home"); }

            await repositorioCuentas.Actualizar(cuentaAEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(id, usuarioID);
            if (cuenta == null) { return RedirectToAction("NoEncontrado", "Home"); }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(id, usuarioID);
            if (cuenta == null) { return RedirectToAction("NoEncontrado", "Home"); }

            await repositorioCuentas.Borrar(id);

            return RedirectToAction("Index");
        }

    }
}
