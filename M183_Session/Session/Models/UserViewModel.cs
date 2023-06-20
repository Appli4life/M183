using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Session.Models
{
    public class UserViewModel
    {
        [MinLength(3, ErrorMessage = "Benutzername zu kurz: min {1}")]
        [MaxLength(50, ErrorMessage = "Benutzername ist zu lang: max {1}")]
        [Required(ErrorMessage = "Der Benutzername ist Notwendig")]
        [DisplayName("Benutzername")]
        public string UserName { get; set; }

        [MaxLength(255)]
        [DisplayName("Passwort")]
        [Required]
        public string UserPassword { get; set; }

    }
}
