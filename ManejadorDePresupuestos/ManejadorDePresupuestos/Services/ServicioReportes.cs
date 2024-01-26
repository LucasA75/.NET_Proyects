using ManejadorDePresupuestos.Models;

namespace ManejadorDePresupuestos.Services
{
    public interface IServicioReportes
    {
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioID, int mes, int ano, dynamic viewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerTransaccionesDetalladasPorCuenta(int usuarioID, int cuentaID, int mes, int ano, dynamic viewBag);
    }

    public class ServicioReportes: IServicioReportes
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly HttpContext HttpContext;

        public ServicioReportes(IRepositorioTransacciones repositorioTransacciones, IHttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.HttpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemanal(int usuarioID,int mes, int ano, dynamic viewBag)
        {
            (DateTime fechaInicio, DateTime fechaFinal) = GenerarFechaInicioYFin(mes, ano);


            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                usuarioID = usuarioID,
                FechaFinal = fechaFinal,
                FechaInicio = fechaInicio,
            };

            AsignarValoresAlViewBag(viewBag, fechaInicio);

            var modelo = await repositorioTransacciones.ObtenerPorSemana(parametro);
            return modelo;
        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioID,int mes,int ano, dynamic viewBag)
        {
            (DateTime fechaInicio, DateTime fechaFinal) = GenerarFechaInicioYFin(mes, ano);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                usuarioID = usuarioID,
                FechaFinal = fechaFinal,
                FechaInicio = fechaInicio,
            };

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioID(parametro);

            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFinal, transacciones);
            AsignarValoresAlViewBag(viewBag, fechaInicio);
            return modelo;
        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerTransaccionesDetalladasPorCuenta(int usuarioID, int cuentaID, int mes, int ano, dynamic viewBag)
        {
            (DateTime fechaInicio, DateTime fechaFinal) = GenerarFechaInicioYFin(mes, ano);

            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                CuentaID = cuentaID,
                UsuarioID = usuarioID,
                FechaFinal = fechaFinal,
                FechaInicio = fechaInicio,
            };

            var transacciones = await repositorioTransacciones.ObtenerPorCuentaID(obtenerTransaccionesPorCuenta);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFinal, transacciones);

            AsignarValoresAlViewBag(viewBag, fechaInicio);

            return modelo;
        }

        private void AsignarValoresAlViewBag(dynamic viewBag, DateTime fechaInicio)
        {
            viewBag.anoAnterior = fechaInicio.AddMonths(-1).Year;
            viewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            viewBag.anoPosterior = fechaInicio.AddMonths(1).Year;
            viewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            viewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetalladas GenerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime fechaFinal, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransaccionesDetalladas();

            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
                {
                    fechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable(),
                });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFinal;
            return modelo;
        }

        private (DateTime fechaInicio, DateTime fechaFinal) GenerarFechaInicioYFin(int mes, int ano)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || ano <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(ano, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}
