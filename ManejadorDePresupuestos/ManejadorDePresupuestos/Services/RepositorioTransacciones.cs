using AutoMapper.Configuration.Conventions;
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
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioID, int ano);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario cuenta);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioID(ParametroObtenerTransaccionesPorUsuario cuenta);
        Task<Transaccion> ObtenerTransaccionPorID(int ID, int usuarioID);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connection;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connection = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioID, int ano)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryAsync<ResultadoObtenerPorMes>(@"SELECT MONTH(FechaTransaccion) as Mes, SUM(Monto) as Monto, cat.TipoOperacionID 
                    FROM Transacciones
                    INNER JOIN Categorias cat
                    ON cat.ID = Transacciones.CategoriaID
                    WHERE Transacciones.UsuarioID = @UsuarioID AND Year(FechaTransaccion) = @ano
                    GROUP BY MONTH(FechaTransaccion), cat.TipoOperacionID", new {usuarioID, ano});

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

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioID(ParametroObtenerTransaccionesPorUsuario cuenta)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryAsync<Transaccion>(@"SELECT t.id, t.Monto, t.FechaTransaccion, ct.Nombre as Categoria, cu.Nombre as Cuenta,
                ct.TipoOperacionID,Nota
                FROM Transacciones t
                INNER JOIN Categorias ct
                ON ct.ID = t.CategoriaID
                INNER JOIN Cuentas cu
                ON cu.ID = t.CuentaID
                WHERE t.UsuarioID = @UsuarioID 
                AND FechaTransaccion Between @FechaInicio AND @FechaFinal
                ORDER BY t.FechaTransaccion DESC
                ", cuenta);

        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario cuenta)
        {
            using var connect = new SqlConnection(connection);
            return await connect.QueryAsync<ResultadoObtenerPorSemana>(@"
                SELECT datediff(d,@fechaInicio,FechaTransaccion) / 7 + 1 as Semana, Sum(Monto) as Monto, ct.TipoOperacionID
                From Transacciones
                INNER JOIN Categorias ct
                ON ct.ID = Transacciones.CategoriaID
                Where Transacciones.UsuarioID = @UsuarioID AND
                FechaTransaccion Between @fechaInicio and @fechaFinal
                Group BY datediff(d,@fechaInicio,FechaTransaccion) / 7, ct.TipoOperacionID
                ", cuenta);
        }

        public async Task Borrar(int ID)
        {
            using var connect = new SqlConnection(connection);
            await connect.ExecuteAsync(@"Transacciones_Borrar", new { ID }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
