using APP_Presupuesto.Models;
using System.Threading.Tasks;

namespace APP_Presupuesto.Interfaces.Repositorios
{
    public interface IRepositorioTipoCuenta
    {

        Task Crear(Tipocuenta tipocuenta);
        Task<bool> ValidarExistencia(string nombre, int usuarioID);
        Task<IEnumerable<Tipocuenta>> Obtener(int usuarioid);
        Task<Tipocuenta> obtenerPorId(int id, int usuarioId);
        Task Actualizar(Tipocuenta tipocuenta);
        Task Borrar(int id);
        Task OrdenarCuentas(IEnumerable<Tipocuenta> TipoCuentasOrdenados);
        Task<bool> TieneDependencias(int id);
    }
}
