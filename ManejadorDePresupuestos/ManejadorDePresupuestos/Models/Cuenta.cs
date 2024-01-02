using ManejadorDePresupuestos.Validations;
using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Models
{
    public class Cuenta
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(50,ErrorMessage ="El largo maximo a sido sobrepasado")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public int ID { get; set; }
        [Display(Name = "Tipo Cuenta")]
        public int TipoCuentaID { get; set; }
        public decimal Balance { get; set; }
        [StringLength(1000, ErrorMessage = "El largo maximo a sido sobrepasado")]
        public string Descripcion { get; set; }
        public string TipoCuenta { get; set; }
    }
}
