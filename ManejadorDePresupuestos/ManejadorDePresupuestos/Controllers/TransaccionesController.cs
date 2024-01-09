using ManejadorDePresupuestos.Models;
using ManejadorDePresupuestos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejadorDePresupuestos.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;

        public TransaccionesController(
            IRepositorioTransacciones repositorioTransacciones, 
            IServicioUsuarios servicioUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategorias repositorioCategorias
            )
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public async Task<ActionResult> Crear()
        {
            var userID = servicioUsuarios.ObtenerUsuarioID();
            var transaccion = new TransaccionCreacionViewModel();
            transaccion.UsuarioID = userID;
            transaccion.Categoria = await ObtenerCategoria(userID, transaccion.TipoOperacion);
            transaccion.Cuenta = await this.ObtenerCuentas(userID);
           
            return View(transaccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(TransaccionCreacionViewModel transaccion)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            transaccion.UsuarioID = usuarioID;
            if (!ModelState.IsValid) {
                transaccion.Categoria = await ObtenerCategoria(usuarioID,transaccion.TipoOperacion);
                transaccion.Cuenta = await ObtenerCuentas(usuarioID);
                return View(transaccion); 
            }

            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(transaccion.CuentaID, usuarioID);
            if(cuenta is null) { return RedirectToAction("NoEncontrado", "Home"); }

            var categoria = await repositorioCategorias.ObtenerPorID(transaccion.CategoriaID, usuarioID);
            if (categoria is null) { return RedirectToAction("NoEncontrado", "Home"); }

            if(transaccion.TipoOperacion == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await repositorioTransacciones.Crear(transaccion);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int id)
        {
            var cuentas = await repositorioCuentas.Buscar(id);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.ID.ToString()));
        }        
        private async Task<IEnumerable<SelectListItem>> ObtenerCategoria(int usuarioID, TipoOperacion tipoOperacion)
        {
            var categoria = await repositorioCategorias.Obtener(usuarioID, tipoOperacion);
            return categoria.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var categorias = await repositorioCategorias.Obtener(usuarioID, tipoOperacion);
            return Ok(categorias);
        }
    }
}
