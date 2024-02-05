using ManejadorDePresupuestos.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejadorDePresupuestos.Models
{
    public class TipoCuenta //:IValidatableObject
    {
        public int ID { get; set; }
        [Required (ErrorMessage = "Es requerido el campo {0}")]
        [StringLength (50,MinimumLength = 3,ErrorMessage ="Longitud de {0} no esta entre {2} y {1}")]
        [Display(Name = "Nombre tipo de cuenta")]
        [PrimeraLetraMayuscula]
        [Remote(controller: "TipoCuentas", action: "ExistsTipoCuenta",AdditionalFields ="ID")]
        public string Nombre { get; set; }
        public int UsuarioID { get; set; }
        public int Orden { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Nombre != null && Nombre.Length > 0) {

        //        var primeraLetra = Nombre[0].ToString();

        //        if(primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra no es mayuscula", new[] {nameof(Nombre)});
        //        }

        //    }
        //}

        /*
         
         
        // Pruebas de otras validacioes por defecto
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="Ingresa un Email Correcto")]
        public string Email { get; set; }
        [Range(minimum:18, maximum:130,ErrorMessage = "Ingresa una {0} entre {1} y {2}")]
        public int Edad { get; set; }
        [Url(ErrorMessage ="El campo debe ser una URL valida")]
        public string URL { get; set; }
        [CreditCard(ErrorMessage = "La tarjeta de credito no es valida")]
        [Display(Name = "Tarjeta de credito")]
        public string TarjetaDeCredito { get; set; }
         
         */

    }
}
