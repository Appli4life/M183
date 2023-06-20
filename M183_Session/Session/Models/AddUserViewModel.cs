using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Session.Models
{
    public class AddUserViewModel
    {
        [MinLength(3, ErrorMessage = "Benutzername ist zu kurz: min {1}")]
        [MaxLength(50, ErrorMessage = "Benutzername ist zu lang: max {1}")]
        [Required(ErrorMessage = "Der Benutzername ist Notwendig")]
        [DisplayName("Benutername")]
        public string UserName { get; set; }

        [MinLength(8, ErrorMessage = "Passwort ist zu kurz: min {1}")]
        [MaxLength(255, ErrorMessage = "Passwort ist zu lang: max {1}")]
        [DisplayName("Passwort")]
        [Required(ErrorMessage = "Das Passwort ist Notwendig")]
        public string UserPassword { get; set; }

        [DisplayName("Passwort bestätigen")]
        [Compare(nameof(UserPassword), ErrorMessage = "Passwort stimmen nicht überein")]
        [Required(ErrorMessage = "Die Bestätigung ist Notwendig")]
        public string UserSecondPassword { get; set; }

    }
}
