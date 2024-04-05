using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Entity
{
	public class Tasks
	{
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string UsuarioCreacionId { get; set; }
        public IdentityUser UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<Paso> Pasos{ get; set; }
        public List<ArchivoAdjunto> ArchivosAdjuntos { get; set; }
    }
}
