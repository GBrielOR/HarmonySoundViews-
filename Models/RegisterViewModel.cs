using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HarmonySound.MVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un rol")]
        [Display(Name = "Rol")]
        public string Role { get; set; }

        // NO debe tener [Required]
        public IEnumerable<string> Roles { get; set; }
    }
}