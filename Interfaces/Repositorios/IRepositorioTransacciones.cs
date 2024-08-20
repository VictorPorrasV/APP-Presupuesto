using APP_Presupuesto.Models;

namespace APP_Presupuesto.Interfaces.Repositorios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transacciones transaccion, decimal montoAnterior, int cuentaAnterior);
        Task CrearTransaccion(Transacciones transaccion);
        Task<Transacciones> ObtenerTransaccionPorID(int id, int usuarioId);

        Task Borrar(int id);
        Task<IEnumerable<Transacciones>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<IEnumerable<Transacciones>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
    }
}
