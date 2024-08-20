using APP_Presupuesto.Models;

namespace APP_Presupuesto.Interfaces.Servicios
{
    public interface IServicioReportes
    {
        
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ReporteSemanal(int usuarioId,
            int mes, int año, dynamic ViewBag);
    }
}
