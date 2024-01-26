using Dapper;
using ManejadorDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Services
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int cuentaID);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioID);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerCuentaPorID(int ID, int usuarioID);
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
                @"INSERT INTO Cuentas(Nombre,TipoCuentaID,Descripcion,Balance) " +
                "VALUES(@Nombre,@TipoCuentaID,@Descripcion,@Balance);" +
                "SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.ID = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioID)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryAsync<Cuenta>(@"
            SELECT Cuentas.ID, Cuentas.Nombre, Cuentas.Balance, TipoCuenta.Nombre AS TipoCuenta
            FROM Cuentas
            INNER JOIN TipoCuenta
            ON TipoCuenta.ID = Cuentas.TipoCuentaID
            WHERE TipoCuenta.UsuarioID = @usuarioID
            ORDER BY TipoCuenta.Orden", new {usuarioID});
        }

        public async Task<Cuenta> ObtenerCuentaPorID(int ID, int usuarioID)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryFirstOrDefaultAsync<Cuenta>(@"
            SELECT Cuentas.ID, Cuentas.Nombre, Cuentas.Balance, TipoCuenta.Nombre AS TipoCuenta, descripcion, TipoCuentaID
            FROM Cuentas
            INNER JOIN TipoCuenta
            ON TipoCuenta.ID = Cuentas.TipoCuentaID
            WHERE TipoCuenta.UsuarioID = @usuarioID AND Cuentas.ID = @ID
            ORDER BY TipoCuenta.Orden", new { usuarioID , ID });
        } 

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connect = new SqlConnection(connection);
            await connect.ExecuteAsync(@"UPDATE Cuentas SET Nombre = @Nombre, Balance = @Balance,Descripcion = @Descripcion,TipoCuentaID = @TipoCuentaID WHERE ID = @ID", cuenta);
        }

        public async Task Borrar(int cuentaID)
        {
            using var connect = new SqlConnection(connection);
            await connect.ExecuteAsync(@"DELETE Cuentas WHERE ID = @cuentaID",new {cuentaID});
        }
    }
}
