using Microsoft.EntityFrameworkCore;
using Session.Context.Entity;
using Session.Utilities;

namespace Session.Context
{
    public class SessionDBContext : DbContext
    {
        public DbSet<UserIdentity> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("SessionM182");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Users.Add(new UserIdentity { UserName = "Tom", Role = SessionConstants.UserRole });
            Users.Add(new UserIdentity { UserName = "Friz", Role = SessionConstants.AdminRole });
            SaveChanges();

            base.OnModelCreating(modelBuilder);
        }
    }
}
