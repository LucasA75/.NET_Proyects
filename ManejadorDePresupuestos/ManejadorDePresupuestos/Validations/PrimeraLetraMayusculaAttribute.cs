using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Validations
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeraLetra = value.ToString()![0].ToString();

            if(primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult(ErrorMessage = "La primera letra no es mayuscula");
            }
            return ValidationResult.Success;
        }
    }
}
