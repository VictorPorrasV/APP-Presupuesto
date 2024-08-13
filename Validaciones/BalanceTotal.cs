using APP_Presupuesto.Models;

namespace APP_Presupuesto.Validaciones
{
    public class BalanceTotal
    {
        public static string GetTotalesClass(IEnumerable<IndiceCuentaVM> model)
        {
            var activos = model.Where(x => x.Balance > 0).Sum(x => x.Balance);
            var pasivos = model.Where(x => x.Balance < 0).Sum(x => x.Balance);
            return activos >= -pasivos ? "text-success" : "text-danger";
        }

        public static decimal GetActivos(IEnumerable<IndiceCuentaVM> model)
        {
            return model.Where(x => x.Balance > 0).Sum(x => x.Balance);
        }

        public static decimal GetPasivos(IEnumerable<IndiceCuentaVM> model)
        {
            return model.Where(x => x.Balance < 0).Sum(x => x.Balance);
        }

        public static decimal GetTotales(IEnumerable<IndiceCuentaVM> model)
        {
            return model.Sum(x => x.Balance);
        }
    }
}
