using Chat.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data
{

    public class ChatDbContext : IdentityDbContext<User>
    {
        public ChatDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsRequired(true);
            builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsRequired(true);
        }

        public DbSet<User> User { get; set; }
    }
}
