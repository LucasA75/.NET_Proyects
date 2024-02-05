using Dapper;
using ManejadorDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejadorDePresupuestos.Services
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
		Task Borrar(int id);
        Task<int> Contar(int UsuarioID);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioID, PaginacionViewModel paginacion);
        Task<IEnumerable<Categoria>> Obtener(int usuarioID, TipoOperacion tipoOperacionID);
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

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioID, PaginacionViewModel paginacion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(@$"
                        SELECT * FROM Categorias WHERE UsuarioID = @usuarioID
                        ORDER BY Nombre
                        OFFSET {paginacion.RecordASaltar} ROWS FETCH NEXT {paginacion.RecordsPorPagina}
                        ROWS ONLY", 
                        new { usuarioID });

        }

        public async Task<int> Contar(int UsuarioID)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>(@"
            SELECT COUNT(*) FROM Categorias WHERE UsuarioID = @UsuarioID
            ", new { UsuarioID });
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioID, TipoOperacion tipoOperacionID)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("SELECT * FROM Categorias WHERE UsuarioID = @usuarioID AND TipoOperacionID = @tipoOperacionID", new { usuarioID, tipoOperacionID });

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
