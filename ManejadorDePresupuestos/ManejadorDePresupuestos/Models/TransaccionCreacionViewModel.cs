using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Models
{
    public class TransaccionCreacionViewModel : Transaccion
    {
        public IEnumerable<SelectListItem> Cuenta { get; set; }
        public IEnumerable<SelectListItem> Categoria { get; set; }
    }
}
