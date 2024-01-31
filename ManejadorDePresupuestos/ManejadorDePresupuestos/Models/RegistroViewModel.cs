using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Models
{
    public class RegistroViewModel
    {
        [EmailAddress(ErrorMessage = " El campo debe ser un email valido")]
        [Required(ErrorMessage = " El Campo {0} es requerido")]
        public string Email { get; set; }
        [Required(ErrorMessage = " El Campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
