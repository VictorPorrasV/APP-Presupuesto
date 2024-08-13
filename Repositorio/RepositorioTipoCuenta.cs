using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
namespace APP_Presupuesto.Repositorio
{
    public class RepositorioTipoCuenta : IRepositorioTipoCuenta
    {

        private readonly string connectionString;
        public RepositorioTipoCuenta(IConfiguration configuration)
        {

            connectionString = configuration.GetConnectionString("Conexion");

        }
        public async Task Crear(Tipocuenta tipocuenta)
        {

            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                            "TiposCuentas_Insertar"
                            , new
                            {
                                usuarioId = tipocuenta.UsuarioId,
                                nombre = tipocuenta.Nombre
                            },
                            commandType:System.Data.CommandType.StoredProcedure) ;

            tipocuenta.id = id; 

        }
        public async Task<bool> ValidarExistencia (string nombre , int usuarioID)
        {

            using var connection = new SqlConnection(connectionString);
            var existencia = await connection.QueryFirstOrDefaultAsync<int>(
                                $@"Select 1
                                from TiposCuentas 
                                Where Nombre =@Nombre AND UsuarioId= @UsuarioiD; ",

                                new { nombre, usuarioID });
                
                return existencia ==1;
        }
        public async Task<IEnumerable<Tipocuenta>> Obtener(int usuarioid)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Tipocuenta>(
                            @"Select Id,Nombre , Orden
                            from TiposCuentas 
                            where UsuarioID= @usuarioId
                            ORDER By Orden", new { usuarioid });
        }
        public async Task Actualizar (Tipocuenta tipocuenta)
        {
            using var connection = new SqlConnection (connectionString);

            await connection.ExecuteAsync(@"Update Tiposcuentas 
                                            Set Nombre = @Nombre Where id=@id", tipocuenta);
        }
        public async Task<Tipocuenta> obtenerPorId(int id,int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Tipocuenta>(
                                     @"Select Id,Nombre, Orden
                                     from TiposCuentas 
                                     where id=@id And usuarioId=@UsuarioId", new {id,usuarioId});
        }
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Delete TiposCuentas where id = @id", new {id});
        }
        public async Task OrdenarCuentas(IEnumerable<Tipocuenta> TipoCuentasOrdenados)
        {
            var query = "Update Tiposcuentas SET Orden = @Orden where id = @Id;";
            using var connection = new SqlConnection(connectionString);
            foreach (var tipoCuenta in TipoCuentasOrdenados)
            {
                await connection.ExecuteAsync(query, new { Orden = tipoCuenta.Orden, Id = tipoCuenta.id });
            }
        }

        public async Task<bool> TieneDependencias(int id)
        {
            // Consulta para verificar si existen cuentas relacionadas
            string query = "SELECT COUNT(*) FROM Cuentas WHERE TipoCuentaId = @id"; // Reemplaza 'OtraTabla' con el nombre de tu tabla y 'CuentaId' con la columna que hace referencia a la cuenta
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                int count = await connection.ExecuteScalarAsync<int>(query, new { id = id });
                return count > 0;
            }
        }
    }


}
