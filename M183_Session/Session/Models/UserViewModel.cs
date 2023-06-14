using System.ComponentModel.DataAnnotations;

namespace Session.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [MaxLength(255), MinLength(5), Required]
        public string UserName { get; set; }

        [MaxLength(255), MinLength(8), Required]
        public string UserPassword { get; set; }

    }
}
