using Dapper;
using ManejadorDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Services
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;

        public RepositorioUsuarios(IConfiguration config) {
            this.connectionString = config.GetConnectionString("DefaultConnection");
        }   

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var usuarioID = await connection.QuerySingleAsync<int>(@"
                    INSERT INTO
                    Usuarios(Email,EmailNormalizado,PasswordHash)
                    VALUES (@Email,@EmailNormalizado,@PasswordHash);
                    SELECT SCOPE_IDENTITY();
                    ",usuario);

            await connection.ExecuteAsync("CrearDatosUsuarioNuevo", new {usuarioID},
                commandType: System.Data.CommandType.StoredProcedure);

            return usuarioID;
        }





        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                @"SELECT * FROM Usuarios WHERE EmailNormalizado = @emailNormalizado",
                new { emailNormalizado });
        }
    }
}
