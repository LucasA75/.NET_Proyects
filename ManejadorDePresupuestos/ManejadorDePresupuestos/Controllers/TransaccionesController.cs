﻿using AutoMapper;
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
		private readonly IMapper mapper;

		public TransaccionesController(
            IRepositorioTransacciones repositorioTransacciones, 
            IServicioUsuarios servicioUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategorias repositorioCategorias,
            IMapper mapper
            )
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
			this.mapper = mapper;
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
            transaccion.Categoria = await ObtenerCategoria(userID, transaccion.TipoOperacionID);
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
                transaccion.Categoria = await ObtenerCategoria(usuarioID,transaccion.TipoOperacionID);
                transaccion.Cuenta = await ObtenerCuentas(usuarioID);
                return View(transaccion); 
            }

            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(transaccion.CuentaID, usuarioID);
            if(cuenta is null) { return RedirectToAction("NoEncontrado", "Home"); }

            var categoria = await repositorioCategorias.ObtenerPorID(transaccion.CategoriaID, usuarioID);
            if (categoria is null) { return RedirectToAction("NoEncontrado", "Home"); }

            if(transaccion.TipoOperacionID == TipoOperacion.Gasto)
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
        public async Task<ActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var transacciones = await repositorioTransacciones.ObtenerTransaccionPorID(id,usuarioID);

            if(transacciones is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transacciones);

            modelo.MontoAnterior = modelo.Monto;

            if(modelo.TipoOperacionID == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.UrlRetorno= urlRetorno;
			modelo.CuentaIDAnterior = transacciones.CuentaID;
            modelo.Categoria = await ObtenerCategoria(usuarioID, modelo.TipoOperacionID);
            modelo.Cuenta = await ObtenerCuentas(usuarioID);


            return View(modelo);
        }

		[HttpPost]
		public async Task<ActionResult> Editar(TransaccionActualizacionViewModel modelo)
		{
			var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            if (!ModelState.IsValid)
            {
				modelo.Categoria = await ObtenerCategoria(usuarioID, modelo.TipoOperacionID);
				modelo.Cuenta = await ObtenerCuentas(usuarioID);
                return View(modelo);
			}

            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(modelo.CuentaID, usuarioID);
            if(cuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorID(modelo.CategoriaID, usuarioID);
            if(categoria == null)
            {
				return RedirectToAction("NoEncontrado", "Home");
			}

            var transaccion = mapper.Map<Transaccion>(modelo);

            if (modelo.TipoOperacionID == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await repositorioTransacciones.Actualizar(modelo, modelo.MontoAnterior, modelo.CuentaIDAnterior);

            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
            return RedirectToAction("Index");

            }
            else
            {
            return LocalRedirect(modelo.UrlRetorno);
            }
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

        [HttpPost]
        public async Task<ActionResult> Borrar(int id)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var transaccion = await repositorioTransacciones.ObtenerTransaccionPorID(id, usuarioID);

            if(transaccion == null) { return RedirectToAction("NoEncontrado", "Home"); }

            await repositorioTransacciones.Borrar(id);

            return RedirectToAction("Index");
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
