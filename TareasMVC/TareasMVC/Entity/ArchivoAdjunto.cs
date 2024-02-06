using Microsoft.EntityFrameworkCore;

namespace TareasMVC.Entity
{
	public class ArchivoAdjunto
	{
        public Guid ID { get; set; }
        public int TasksID { get; set; }
        public Tasks Task { get; set; }
        [Unicode]
        public string URL { get; set; }
        public string Titulo { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion{ get; set; }
    }
}
