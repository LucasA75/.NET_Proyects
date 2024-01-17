namespace ManejadorDePresupuestos.Models
{
    public class ObtenerTransaccionesPorCuenta
    {
        public int UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
    }
}
