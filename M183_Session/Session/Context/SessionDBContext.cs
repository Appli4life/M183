using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Session.Context.Entity;
using Session.Utilities;

namespace Session.Context
{
    public class SessionDBContext : DbContext
    {
        public SessionDBContext()
        {
        }

        public SessionDBContext(DbContextOptions<SessionDBContext> options)
            : base(options) { }

        public DbSet<UserIdentity> Users { get; set; }

        public void Seed()
        {
            var hasher = new PasswordHasher<UserIdentity>();

            var user = new UserIdentity()
            {
                Id = 1,
                UserName = "Simon",
                Password = "Simon",
                Role = SessionConstants.UserRole,
            };

            user.Password = hasher.HashPassword(user, user.Password);

            var adminUser = new UserIdentity()
            {
                Id = 2,
                UserName = "Admin",
                Role = SessionConstants.AdminRole,
                Password = "Admin"
            };

            adminUser.Password = hasher.HashPassword(adminUser, adminUser.Password);

            this.Users.Add(user);
            this.Users.Add(adminUser);
            SaveChanges();

        }
    }
}
