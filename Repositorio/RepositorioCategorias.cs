using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
using System.Data;

namespace APP_Presupuesto.Repositorio
{
    public class RepositorioCategorias :IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Conexion");
            
        }

        public async Task Crear(Categorias categoria)
        {
            using var connnection = new SqlConnection(connectionString);
            var id = await connnection.QuerySingleAsync<int>(
                        "[dbo].[Categorias_Insertar]",
                        new { Nombre = categoria.Nombre, TipoOperacionId = categoria.TipoOperacionId, UsuarioId = categoria.UsuarioId },
                        commandType: CommandType.StoredProcedure);
            categoria.Id = id;

        }
        public async Task<IEnumerable<Categorias>> Obtener(int usuarioid)
        {

            using var connnection = new SqlConnection(connectionString);

            return await connnection.QueryAsync<Categorias>(@"Select * 
                     from categorias where UsuarioId= @Usuarioid
                      ORDER By Orden", new { usuarioid });
        }

        public async Task<IEnumerable<Categorias>> Obtener(int usuarioid,TipoOperaciones TipoOperacionId)
        {

            using var connnection = new SqlConnection(connectionString);

            return await connnection.QueryAsync<Categorias>(@"
                     Select * 
                     from categorias 
                     where UsuarioId= @Usuarioid and TipoOperacionId = @TipoOperacionId
                     ORDER By Orden", new { usuarioid, TipoOperacionId });
        }


        public async Task OrdenarCategorias(IEnumerable<Categorias> CategoriasOrdenados)
        {
            var query = "Update Categorias SET Orden = @Orden where id = @Id;";
            using var connection = new SqlConnection(connectionString);
            foreach (var categoria in CategoriasOrdenados)
            {
                await connection.ExecuteAsync(query, new { orden = categoria.Orden, id = categoria.Id });
            }
        }

        public async Task <Categorias> ObtenerCategoriaPorID(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categorias>(@"Select *
                                                                        from categorias
                                                                        where Id= @id and UsuarioId= @UsuarioId", new { id, usuarioId });
        }

        public async Task ActualizarCategoria(Categorias categorias)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Update Categorias 
                                          set Nombre=@Nombre, TipoOperacionId = @TipoOperacionID
                                          where Id= @Id"
                                           ,  categorias );
        }
    
    
        public async Task BorrarCategoria(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"Delete Categorias Where Id=@Id", new { id });
        }
    }

}
