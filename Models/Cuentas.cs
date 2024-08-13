using APP_Presupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace APP_Presupuesto.Models
{
    public class Cuentas
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "El campo {0} es requerido")]
        [StringLength (maximumLength:50)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        [Display(Name ="Tipo Cuenta")]
        public int TipoCuentaId { get; set; }
       
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [ValidarCampoDecimal]
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Descripcion { get; set; }

        //se utiliza en el repositorioTipoCuenta para realizar el metodo Buscar
        public string TipoCuenta { get; set; }


    }
}
