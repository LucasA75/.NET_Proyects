using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Models
{
	public class RegistroViewModel
	{
        [Required]
        [EmailAddress(ErrorMessage = "Debe ser un email valido")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
