using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace APP_Presupuesto.Models
{
    public class VMTransaccionCreacion:Transacciones
    {
        public IEnumerable<SelectListItem>Cuentas { get; set; }
        public IEnumerable<SelectListItem> Categorias { get; set; }



    }
}
