using APP_Presupuesto.Models;

namespace APP_Presupuesto.Interfaces.Repositorios
{
    public interface IRepositorioCuentas
    {
        Task Crear(Cuentas cuentas);
        Task<IEnumerable<Cuentas>> Buscar(int usuarioid);
        Task<Cuentas> ObtenerCuentaPorID(int id, int usuarioid);
        Task Actualizar(CuentasVM cuentasVm);
        
        Task Borrar(int id);
    }
}
