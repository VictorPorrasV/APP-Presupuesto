using APP_Presupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace APP_Presupuesto.Models
{
    public class Categorias
    {

        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength:50, ErrorMessage ="Este campo no puede ser mayor a {1} caracteres")]
        public string Nombre { get; set; }
        [Display(Name = "Tipo de operacion")]
        public TipoOperaciones TipoOperacionId { get; set; }
        public int UsuarioId { get; set; }

        public int Orden { get; set; }
    }
}
