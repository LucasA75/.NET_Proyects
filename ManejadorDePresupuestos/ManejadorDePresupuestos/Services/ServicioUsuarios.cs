namespace ManejadorDePresupuestos.Services
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioID();
    }

    public class ServicioUsuarios:IServicioUsuarios
    {
        public int ObtenerUsuarioID()
        {
            return 1;
        }
    }
}
