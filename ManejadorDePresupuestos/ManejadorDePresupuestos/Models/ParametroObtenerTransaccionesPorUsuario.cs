namespace ManejadorDePresupuestos.Models
{
    public class ParametroObtenerTransaccionesPorUsuario
    {
        public int usuarioID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
    }
}
