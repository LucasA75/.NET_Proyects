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
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaID(ObtenerTransaccionesPorCuenta cuenta);
        Task<Transaccion> ObtenerTransaccionPorID(int ID, int usuarioID);
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

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaIDAnterior)
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
                cuentaIDAnterior
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaID(ObtenerTransaccionesPorCuenta cuenta)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryAsync<Transaccion>(@"SELECT t.id, t.Monto, t.FechaTransaccion, ct.Nombre as Categoria, cu.Nombre as Cuenta,
                ct.TipoOperacionID 
                FROM Transacciones t
                INNER JOIN Categorias ct
                ON ct.ID = t.CategoriaID
                INNER JOIN Cuentas cu
                ON cu.ID = t.CuentaID
                WHERE t.CuentaID = @CuentaID AND t.UsuarioID = @UsuarioID 
                AND FechaTransaccion Between @FechaInicio AND @FechaFinal", cuenta);
        }

        public async Task Borrar(int ID)
        {
            using var connect = new SqlConnection(connection);
            await connect.ExecuteAsync(@"Transacciones_Borrar", new { ID },commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
