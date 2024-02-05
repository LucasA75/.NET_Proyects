using Dapper;
using ManejadorDePresupuestos.Models;
using ManejadorDePresupuestos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Controllers
{
    public class TipoCuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TipoCuentasController(IRepositorioTipoCuentas repositorioTipoCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var tiposCuentas = await repositorioTipoCuentas.Obtener(usuarioID);
            return View(tiposCuentas);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorID(id,usuarioID);

            if(tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioUD = servicioUsuarios.ObtenerUsuarioID();
            var tipoCuentaExiste = await repositorioTipoCuentas.ObtenerPorID(tipoCuenta.ID, usuarioUD);
            if(tipoCuentaExiste is null) { return RedirectToAction("NoEncontrado", "Home"); }

            await repositorioTipoCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if(!ModelState.IsValid) { return  View(tipoCuenta); }
            
            tipoCuenta.UsuarioID = servicioUsuarios.ObtenerUsuarioID();

            var existeNombreCuenta = await repositorioTipoCuentas.Exists(tipoCuenta.Nombre, tipoCuenta.UsuarioID);
            if (existeNombreCuenta) {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe" );

                return View(tipoCuenta);
                    }

            await repositorioTipoCuentas.Create(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExistsTipoCuenta(string nombre,int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();

            var existe = await repositorioTipoCuentas.Exists(nombre, usuarioID, id);

            if (existe)
            {
                return Json($"El tipo de cuenta {nombre} ya existe");
            }
            return Json(true);
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorID(id,usuarioID);
            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorID(id, usuarioID);
            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTipoCuentas.Borrar(id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var tiposCuentas = await repositorioTipoCuentas.Obtener(usuarioID);
            var idsCuentas = tiposCuentas.Select(x => x.ID);

            var idstiposCuentasNoPertenecenAlUsuario = ids.Except(idsCuentas).ToList();

            if(idstiposCuentasNoPertenecenAlUsuario.Count > 0) { return Forbid(); }

            var tiposCuentasOrdenados = ids.Select((valor, indice) => new TipoCuenta() { ID = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTipoCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
