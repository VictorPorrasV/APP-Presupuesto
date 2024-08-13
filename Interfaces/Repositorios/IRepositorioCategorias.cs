using APP_Presupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace APP_Presupuesto.Interfaces.Repositorios
{
    public interface IRepositorioCategorias
    {

        Task Crear(Categorias categoria);
        Task<IEnumerable<Categorias>> Obtener(int usuarioid);
        Task OrdenarCategorias(IEnumerable<Categorias> CategoriasOrdenados);
        Task<Categorias> ObtenerCategoriaPorID(int id, int usuarioId);
        Task ActualizarCategoria(Categorias categorias);
        Task BorrarCategoria(int id);
        Task<IEnumerable<Categorias>> Obtener(int usuarioid, TipoOperaciones tipoOperacionId);
    }
}
