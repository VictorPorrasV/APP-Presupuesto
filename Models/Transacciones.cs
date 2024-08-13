using System.ComponentModel.DataAnnotations;

namespace APP_Presupuesto.Models
{
    public class Transacciones
    {

        public int Id { get; set; } 
        public int UsuarioId { get; set;}
        
        
        [Display(Name = "Fecha Transaccion")]
        [DataType(DataType.DateTime)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("yyy-MM-dd hh:MM tt"));
        
        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La nota es obligatoria.")]
        [StringLength(maximumLength: 1000, ErrorMessage = "La nota no puede exceder los {1} caracteres.")]
        public string Nota { get; set; }



        [Required(ErrorMessage = "Favor seleciona una cuenta.")]
        [Range(0, int.MaxValue, ErrorMessage = "Favor seleciona una cuenta.")]
        [Display(Name = "Cuenta")]
        public int CuentaId { get; set; }



        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }


        //no pertenecen a la tabla 
        [Display(Name = "Tipo de Operacion")]
        //enum  valor default = 1
        public TipoOperaciones TipoOperacionId { get; set; } = TipoOperaciones.Ingreso;

        public string Cuenta { get; set; }
        public string Categoria { get; set; }

        public string Descripcion { get; set; }








    }
}
