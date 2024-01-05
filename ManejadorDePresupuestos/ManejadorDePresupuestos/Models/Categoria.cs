using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50,ErrorMessage ="El campo {0} no puede ser mayor a 50")]
        public string Nombre { get; set; }
        [Display(Name = "Tipo Operacion")]
        public TipoOperacion TipoOperacionID { get; set; }
        public int UsuarioID { get; set; }
    }
}
