namespace ManejadorDePresupuestos.Models
{
    public class PaginacionViewModel
    {
        public int Pagina { get; set; } = 1;
        private int recordsPorPagina = 10;
        private readonly int maxRecordForPages = 50;

        public int RecordsPorPagina
        {
            get{
                return recordsPorPagina;
            }
            set {

                recordsPorPagina = value > maxRecordForPages ? maxRecordForPages : value;
            }
        }

        public int RecordASaltar => recordsPorPagina * (Pagina - 1);

    }
}
