using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Models
{
    public class TipoCuenta
    {
        [Required]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public int UsuarioID { get; set; }
        public int Orden { get; set; }
    }
}
