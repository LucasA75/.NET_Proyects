using System.Collections;

namespace ManejadorDePresupuestos.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(z  => z.BalanceDepositos);
        public decimal BalanceRetiros { get; set; }

        public class TransaccionesPorFecha
        {
            public DateTime fechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }
            public decimal BalanceDepositos => 
                Transacciones.Where(x => x.TipoOperacion == TipoOperacion.Ingreso)
                .Sum(x => x.Monto);
            public decimal BalanceRetiros =>
                Transacciones.Where(x => x.TipoOperacion == TipoOperacion.Gasto)
                .Sum(x => x.Monto);
                    }
            public decimal Total => BalanceRetiros - BalanceDepositos;

    }
}
