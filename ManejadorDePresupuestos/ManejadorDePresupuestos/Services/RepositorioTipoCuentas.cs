using Dapper;
using ManejadorDePresupuestos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Services
{

    public interface IRepositorioTipoCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Create(TipoCuenta tipoCuenta);
        Task<bool> Exists(string nombre, int usuarioID);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioID);
        Task<TipoCuenta> ObtenerPorID(int ID, int usuarioID);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentaOrdenado);
    }
    public class RepositorioTipoCuentas:IRepositorioTipoCuentas
    {
        private readonly string connectionString;

        public RepositorioTipoCuentas(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task Create(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                                        "TipoCuenta_Insertar",
                                        new { Nombre = tipoCuenta.Nombre, UsuarioID = tipoCuenta.UsuarioID }, 
                                        commandType:System.Data.CommandType.StoredProcedure) ;
            tipoCuenta.ID = id;
        }

        public async Task<bool> Exists(string nombre, int usuarioID)
        {
            using var connection = new SqlConnection(connectionString);
            var exits = await connection.QueryFirstOrDefaultAsync<int>
                ($@"SELECT 1 FROM TipoCuenta Where Nombre = @Nombre AND UsuarioID = @UsuarioID", 
                new { nombre, usuarioID});

            return exits == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioID)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT ID,Nombre,UsuarioID,Orden FROM TipoCuenta WHERE UsuarioID = @UsuarioID ORDER BY Orden", new {usuarioID});
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TipoCuenta SET Nombre = @Nombre Where ID = @ID", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorID(int ID, int usuarioID)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT ID,Nombre,UsuarioID,Orden FROM TipoCuenta WHERE UsuarioID = @UsuarioID AND ID = @ID", new { ID, usuarioID });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE TipoCuenta Where ID = @id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentaOrdenado)
        {
            var query = "UPDATE TipoCuenta SET Orden = @Orden WHERE ID = @ID";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentaOrdenado);
        }
    }
}
