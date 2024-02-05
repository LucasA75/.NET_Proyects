using ManejadorDePresupuestos.Models;
using ManejadorDePresupuestos.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManejadorDePresupuestos.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategoria;
        private readonly IServicioUsuarios servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategoria, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioCategoria = repositorioCategoria;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public IActionResult Crear() { 
        
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            if (!ModelState.IsValid) { return View(categoria); }

            categoria.UsuarioID = usuarioID;
            await repositorioCategoria.Crear(categoria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(PaginacionViewModel paginacion)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var categorias = await repositorioCategoria.Obtener(usuarioID, paginacion);
            var totalCategorias = await repositorioCategoria.Contar(usuarioID);

            var respuestaVM = new PaginacionRespuesta<Categoria>()
            {
                Elementos = categorias,
                Pagina = paginacion.Pagina,
                RecordsPorPagina= paginacion.RecordsPorPagina,
                CantidadTotalRecords=totalCategorias,
                BaseURL= Url.Action()

            };

            return View(respuestaVM);
        }

		public async Task<IActionResult> Editar(int id)
		{
			var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var categoriaAEditar = await repositorioCategoria.ObtenerPorID(id, usuarioID);

            if(categoriaAEditar is null) { return RedirectToAction("NoEncontrado", "Home"); }
            
			return View(categoriaAEditar);
		}

        [HttpPost]
		public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
			var usuarioID = servicioUsuarios.ObtenerUsuarioID();
			var categoria = await repositorioCategoria.ObtenerPorID(categoriaEditar.Id, usuarioID);


			if (categoria is null) { return RedirectToAction("NoEncontrado", "Home"); }
			if (!ModelState.IsValid) { return View(categoriaEditar); }

			categoriaEditar.UsuarioID = usuarioID;
            await repositorioCategoria.Actualizar(categoriaEditar);
            return RedirectToAction("Index");

		}

        [HttpGet]
		public async Task<IActionResult> Borrar(int id)
		{
			var usuarioID = servicioUsuarios.ObtenerUsuarioID();
			var categoriaAEliminar = await repositorioCategoria.ObtenerPorID(id, usuarioID);

			if (categoriaAEliminar is null) { return RedirectToAction("NoEncontrado", "Home"); }

			return View(categoriaAEliminar);
		}

		[HttpPost]
		public async Task<IActionResult> BorrarCategoria(int id)
		{
			var usuarioID = servicioUsuarios.ObtenerUsuarioID();
			var categoria = await repositorioCategoria.ObtenerPorID(id, usuarioID);


			if (categoria is null) { return RedirectToAction("NoEncontrado", "Home"); }
			if (!ModelState.IsValid) { return View(categoria); }

			await repositorioCategoria.Borrar(id);
			return RedirectToAction("Index");

		}
	}
}
