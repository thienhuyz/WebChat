using Chat.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<User> User { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }

    }
}
