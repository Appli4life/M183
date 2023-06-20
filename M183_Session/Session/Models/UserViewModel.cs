using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Session.Models
{
    public class UserViewModel
    {

        [MaxLength(50)]
        [Required(ErrorMessage = "Der Benutzername ist Notwendig")]
        [DisplayName("Benutername")]
        public string UserName { get; set; }

        [MaxLength(255)]
        [DisplayName("Passwort")]
        [Required]
        public string UserPassword { get; set; }

    }
}
