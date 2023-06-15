using System.ComponentModel.DataAnnotations;

namespace Session.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(255)]
        [Required]
        public string UserPassword { get; set; }

    }
}
