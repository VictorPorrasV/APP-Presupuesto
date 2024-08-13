using System.ComponentModel.DataAnnotations;

namespace APP_Presupuesto.Validaciones
{
    public class ValidarCampoDecimal : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("El campo Balance es requerido. Favor de ingresar un número decimal o entero válido.");
            }

            // Convertir el valor a string y comprobar si contiene la letra 'e'
            string valorString = value.ToString();
            if (valorString.Contains('e'))
            {
                return new ValidationResult("El campo Balance no puede contener la letra 'e'. Favor de ingresar un número decimal o entero válido.");
            }

            // Intentar convertir el valor a decimal
            if (decimal.TryParse(valorString, out decimal result))
            {
                return ValidationResult.Success; // El valor es un número decimal válido
            }
            else
            {
                return new ValidationResult("Favor de ingresar un número decimal válido para el campo " + validationContext.DisplayName);
            }
        }
    }
}
