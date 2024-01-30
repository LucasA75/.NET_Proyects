using AutoMapper;
using ManejadorDePresupuestos.Models;
using ManejadorDePresupuestos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejadorDePresupuestos.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioReportes servicioReportes;
        private readonly IMapper mapper;

        public TransaccionesController(
            IRepositorioTransacciones repositorioTransacciones,
            IServicioUsuarios servicioUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategorias repositorioCategorias,
            IServicioReportes servicioReportes,
            IMapper mapper
            )
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.servicioReportes = servicioReportes;
            this.mapper = mapper;
        }

        public async Task<ActionResult> Index(int mes, int ano)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioID, mes, ano, ViewBag);
            return View(modelo);
        }
        public async Task<ActionResult> Semanal(int mes,int ano)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana = 
                await servicioReportes.ObtenerReporteSemanal(usuarioID,mes,ano,ViewBag);
            
            var agrupado = transaccionesPorSemana.GroupBy(x=> x.Semana).Select(x => new ResultadoObtenerPorSemana()
            {
                Semana = x.Key,
                Ingresos = x.Where(x=>x.TipoOperacionID == TipoOperacion.Ingreso).Select(x =>x.Monto).FirstOrDefault(),
                Gastos = x.Where(x=>x.TipoOperacionID == TipoOperacion.Gasto).Select(x =>x.Monto).FirstOrDefault(),

            }).ToList();

            if(ano == 0 || mes == 0)
            {
                var hoy = DateTime.Today;
                ano = hoy.Year;
                mes = hoy.Month;
            }

            var fechaReferencia = new DateTime(ano, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);

            var diasSegmentados = diasDelMes.Chunk(7).ToList();

            for(int i = 0; i < diasSegmentados.Count(); i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(ano, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(ano, mes, diasSegmentados[i].Last());
                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                if(grupoSemana == null)
                {
                    agrupado.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,

                    });
                }
                else
                {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }
            }

            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();

            var modelo = new ReporteSemanalViewModel();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaReferencia = fechaReferencia;


            return View(modelo);
        }
        public async Task<ActionResult> Mensual(int ano)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            if (ano == 0)
            {
                ano = DateTime.Today.Year;
            }

            var transaccionesPorMes = await repositorioTransacciones.ObtenerPorMes(usuarioID, ano);

            var transaccionesAgrupadas = transaccionesPorMes.GroupBy(x => x.Mes)
                .Select(x => new ResultadoObtenerPorMes()
                {
                    Mes = x.Key,
                    Ingreso = x.Where(x => x.TipoOperacionID == TipoOperacion.Ingreso).Select(x => x.Monto).FirstOrDefault(),
                    Gasto = x.Where(x => x.TipoOperacionID == TipoOperacion.Gasto).Select(x => x.Monto).FirstOrDefault(),

                }).ToList();

            for(int mes = 12; mes <=12; mes++)
            {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x=> x.Mes == mes);
                var fechaReferencia = new DateTime(ano, mes, 1);
                if(transaccion is null)
                {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes()
                    {
                        Mes = mes,
                        FechaReferencia = fechaReferencia,

                    });
                }
                else
                {
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }

            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();

            var modelo = new ReporteMensualViewModel();
            modelo.Ano = ano;
            modelo.TransaccionesPorMes = transaccionesAgrupadas;

            return View(modelo);
        }
        public async Task<ActionResult> ReporteExcel(int id)
        {
            return View();
        }
        public async Task<ActionResult> Calendario(int id)
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
            if (!ModelState.IsValid)
            {
                transaccion.Categoria = await ObtenerCategoria(usuarioID, transaccion.TipoOperacionID);
                transaccion.Cuenta = await ObtenerCuentas(usuarioID);
                return View(transaccion);
            }

            var cuenta = await repositorioCuentas.ObtenerCuentaPorID(transaccion.CuentaID, usuarioID);
            if (cuenta is null) { return RedirectToAction("NoEncontrado", "Home"); }

            var categoria = await repositorioCategorias.ObtenerPorID(transaccion.CategoriaID, usuarioID);
            if (categoria is null) { return RedirectToAction("NoEncontrado", "Home"); }

            if (transaccion.TipoOperacionID == TipoOperacion.Gasto)
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
            var transacciones = await repositorioTransacciones.ObtenerTransaccionPorID(id, usuarioID);

            if (transacciones is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transacciones);

            modelo.MontoAnterior = modelo.Monto;

            if (modelo.TipoOperacionID == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.UrlRetorno = urlRetorno;
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
            if (cuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorID(modelo.CategoriaID, usuarioID);
            if (categoria == null)
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
        public async Task<ActionResult> Borrar(int id, string urlRetorno = null)
        {
            var usuarioID = servicioUsuarios.ObtenerUsuarioID();
            var transaccion = await repositorioTransacciones.ObtenerTransaccionPorID(id, usuarioID);

            if (transaccion == null) { return RedirectToAction("NoEncontrado", "Home"); }

            await repositorioTransacciones.Borrar(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");

            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
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
