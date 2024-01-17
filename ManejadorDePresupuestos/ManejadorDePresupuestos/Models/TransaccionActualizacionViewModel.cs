namespace ManejadorDePresupuestos.Models
{
	public class TransaccionActualizacionViewModel: TransaccionCreacionViewModel
	{
        public int CuentaIDAnterior { get; set; }
        public decimal MontoAnterior { get; set; }
        public string UrlRetorno {  get; set; }
    }
}
