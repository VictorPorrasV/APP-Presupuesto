using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace APP_Presupuesto.Repositorio
{
    public class RepositorioCuentas: IRepositorioCuentas
    {

        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration) {

            connectionString = configuration.GetConnectionString("Conexion");

        }

        public async Task Crear(Cuentas cuentas)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                                    @"Insert into Cuentas 
                                    (Nombre, TipoCuentaId,Descripcion,Balance)
                                    Values (@Nombre, @TipoCuentaId,@Descripcion,@Balance);
                                    Select SCOPE_IDENTITY();",cuentas);

            cuentas.Id=id;
        }

        public async Task<IEnumerable<Cuentas>> Buscar(int usuarioid)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuentas>(
                                    @"select Cuentas.Id,Cuentas.Nombre,Balance,Descripcion,Tc.Nombre AS TipoCuenta 
                                    from cuentas 
                                    inner join TiposCuentas tc on tc.Id = cuentas.TipoCuentaId
                                    where tc.UsuarioId= @UsuarioId
                                    Order By tc.Orden", new { usuarioid });
        }
        public async Task<Cuentas> ObtenerCuentaPorID(int id,int usuarioid)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuentas>(
                                 @"select Cuentas.Id,Cuentas.Nombre,Balance,Descripcion,Tc.id
                                    from cuentas 
                                    inner join TiposCuentas tc on tc.Id = cuentas.TipoCuentaId
                                    where tc.UsuarioId= @usuarioid and Cuentas.Id=@id", new { id,usuarioid });
        }
        public async Task Actualizar(CuentasVM cuentasVm)
        {

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Update Cuentas Set Nombre= @Nombre, 
                                            Balance= @Balance,
                                            Descripcion = @Descripcion,
                                            TipoCuentaId= @TipoCuentaId where id =@id;",  cuentasVm );
        }

        public async Task Borrar(int id)
        {

            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"Delete Cuentas where id =@id;", new { id });
        }

      
    }
}
