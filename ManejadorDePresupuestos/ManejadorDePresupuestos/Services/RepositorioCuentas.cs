using Dapper;
using ManejadorDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Services
{
    public interface IRepositorioCuentas
    {
        Task Crear(Cuenta cuenta);
    }

    public class RepositorioCuentas : IRepositorioCuentas
    {

        private readonly string connection;

        public RepositorioCuentas(IConfiguration config)
        {
            connection = config.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connect = new SqlConnection(connection);
            var id = await connect.QuerySingleAsync<int>(
                "INSERT INTO Cuenta(Nombre,TipoCuentaID,Descripcion,Balance) " +
                "VALUES(@Nombre,@TipoCuentaID,@Descripcion,@Balance);" +
                "SELECT SCOPRE_IDENTITY();",cuenta);

            cuenta.ID = id;


        }
    }
}
