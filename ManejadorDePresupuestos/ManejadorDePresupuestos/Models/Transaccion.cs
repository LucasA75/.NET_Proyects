using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Campo {0} necesario")]
        [Display(Name ="Fecha Transaccion")]
        [DataType(DataType.DateTime)] 
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy HH:mm tt"));

        [Range(1,maximum:int.MaxValue, ErrorMessage ="Ingresa un monto permitido")]
        public decimal Monto { get; set; }

        [StringLength(1000,ErrorMessage = "La nota no puede acceder los {1}")]
        public string Nota { get; set; }

        [Required(ErrorMessage = "El campo {0} es necesario")]
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El campo {0} es necesario")]
        public int CuentaID { get; set; }

        [Required(ErrorMessage = "El campo {0} es necesario")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debes seleccionar una categoria ")]
        public int CategoriaID { get; set; }

        public TipoOperacion TipoOperacion { get; set; } = TipoOperacion.Ingreso;
    }
}
