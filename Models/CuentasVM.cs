using Microsoft.AspNetCore.Mvc.Rendering;

namespace APP_Presupuesto.Models
{
    public class CuentasVM :Cuentas
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; } //mapea el valor de 

    }
}
