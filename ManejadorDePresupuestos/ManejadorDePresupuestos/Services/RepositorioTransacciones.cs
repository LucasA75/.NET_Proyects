using Dapper;
using ManejadorDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Services
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int transaccionID);
        Task<IEnumerable<Transaccion>> Buscar(int usuarioID);
        Task Crear(Transaccion transaccion);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connection;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connection = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connect = new SqlConnection(connection);
            var id = await connect.QuerySingleAsync<int>(@"Transacciones_Insertar", new
            {
                transaccion.UsuarioID,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaID,
                transaccion.CuentaID,
                transaccion.Nota
            }, commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;

        }

        public async Task<IEnumerable<Transaccion>> Buscar(int usuarioID)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryAsync<Transaccion>(@"
            SELECT Cuentas.ID, Cuentas.Nombre, Cuentas.Balance, TipoCuenta.Nombre AS TipoCuenta
            FROM Cuentas
            INNER JOIN TipoCuenta
            ON TipoCuenta.ID = Cuentas.TipoCuentaID
            WHERE TipoCuenta.UsuarioID = @usuarioID
            ORDER BY TipoCuenta.Orden", new { usuarioID });
        }

        public async Task<Transaccion> ObtenerTransaccionPorID(int ID, int usuarioID)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryFirstOrDefaultAsync<Transaccion>(@"
            SELECT tr.* , Categorias.TipoOperacionID
            FROM Transacciones tr
            INNER JOIN Categorias
            ON tr.CategoriaID = Categorias.ID
            WHERE tr.id = @ID AND tr.UsuarioID = @usuarioID",
            new { usuarioID, ID });
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior)
        {
            using var connect = new SqlConnection(connection);
            await connect.ExecuteAsync(@"Transacciones_Actualizar", new
            {
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaID,
                transaccion.CuentaID,
                transaccion.Nota,
                montoAnterior,
                cuentaAnterior
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Borrar(int transaccionID)
        {
            using var connect = new SqlConnection(connection);
            await connect.ExecuteAsync(@"DELETE Transacciones WHERE id = @transaccionID", new { transaccionID });
        }
    }
}
