using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace APP_Presupuesto.Repositorio
{
    public class RepositorioTransacciones:IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration) {

            connectionString = configuration.GetConnectionString("Conexion");
            
        }


        public async Task CrearTransaccion(Transacciones transaccion)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(@"Transacciones_Insertar",new { 
            transaccion.UsuarioId,   
            transaccion.FechaTransaccion,
            transaccion.Monto,
            transaccion.CategoriaId,
            transaccion.CuentaId,
            transaccion.Nota}
            ,commandType:System.Data.CommandType.StoredProcedure);
            transaccion.Id=id;

        }


        public async Task Actualizar(Transacciones transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {


           
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.Nota,
                    transaccion.CuentaId,
                    montoAnterior,
                    cuentaAnteriorId

                }, commandType: System.Data.CommandType.StoredProcedure);

        }
        public async Task<Transacciones> ObtenerTransaccionPorID(int id , int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Transacciones>(@" 
                Select Transacciones.*, cat.TipoOperacionId
            
                from transacciones 
                inner Join Categorias cat on cat.Id = Transacciones.CategoriaId
                where transacciones.Id = @Id and Transacciones.UsuarioId= @UsuarioId
            ", new {id,usuarioId});
        }

        public async Task Borrar(int id)
        {

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }


        public async  Task<IEnumerable<Transacciones>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transacciones>(@"
                            
                            select t.id, t.Monto,t.FechaTransaccion,c.Nombre as Categoria, 
                            cu.Nombre as Cuenta ,t.Nota as Descripcion,c.TipoOperacionId


                            from
                            Transacciones t 
                            inner Join Categorias c
                            on c.Id= T.CategoriaId
                            inner Join cuentas cu
                            on cu.Id = t.CuentaId
                            where  t.CuentaId =@cuentaid and t.UsuarioId =  @usuarioId
                            and FechaTransaccion between @FechaInicio and @Fechafin ", modelo);


        }
    }


   
}
