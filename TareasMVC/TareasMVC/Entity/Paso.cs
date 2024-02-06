namespace TareasMVC.Entity
{
	public class Paso
	{
        public Guid ID { get; set; }
        public int TasksID { get; set; }
        public bool Realizado { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public Tasks Task { get; set; }
    }
}
