using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Session.Context.Entity
{
    public class UserIdentity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(20)]
        public string Role { get; set; }

        public int Balance { get; set; }
    }
}
