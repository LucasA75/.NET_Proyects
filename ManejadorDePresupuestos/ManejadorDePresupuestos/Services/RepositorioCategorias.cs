using Dapper;
using ManejadorDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Services
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
		Task Borrar(int id);
		Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioID);
        Task<Categoria> ObtenerPorID(int id, int usuarioID);
    }

    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {

            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
            INSERT INTO Categorias(Nombre,TipoOperacionID,UsuarioID)
            VALUES (@Nombre,@TipoOperacionID,@UsuarioID);
            SELECT SCOPE_IDENTITY();
            ", categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioID)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("SELECT * FROM Categorias WHERE UsuarioID = @usuarioID", new { usuarioID });

        }

        public async Task<Categoria> ObtenerPorID(int id, int usuarioID)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"SELECT * FROM Categorias WHERE UsuarioID = @usuarioID AND ID = @id", new { id, usuarioID });

        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.ExecuteAsync(@"
            UPDATE Categorias
            SET Nombre = @Nombre,TipoOperacionID = @TipoOperacionID,UsuarioID = @UsuarioID
            WHERE ID = @Id
            ", categoria);
        }
		public async Task Borrar(int id)
		{
			using var connection = new SqlConnection(connectionString);
			await connection.ExecuteAsync(@"
            DELETE Categorias
            WHERE ID = @Id
            ", new {id});
		}
	}
}
