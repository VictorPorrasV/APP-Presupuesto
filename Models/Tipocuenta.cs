using APP_Presupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace APP_Presupuesto.Models
{
    public class Tipocuenta
    {

        public int id { get; set; }

        [Required (ErrorMessage ="El campo {0} es requerido")]
        [StringLength (maximumLength:50,MinimumLength =3, ErrorMessage = "La longitud del campo {0} debe estar entre {2} y {1}")]
        [Display(Name = "Nombre del tipo cuenta")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        
        //ayuda a configurar el orden como aparecen las cuentas
        public int Orden  { get; set; }




    }
}
