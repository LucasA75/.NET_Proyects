using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Debe ser un email valido")]
        public string Mail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="Recuerdame")]
        public bool Recuerdame { get; set; }
    }
}
