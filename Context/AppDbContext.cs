using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MakeMeFaster.Entities;
using System;

namespace MakeMeFaster.Context
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=localhost;Encrypt=False;Database=MakeMeFaster;Trusted_Connection=True;Integrated Security=true;MultipleActiveResultSets=true");
            //options.UseSqlServer("Server=SAMER\\SQLEXPRESS;Encrypt=False;Database=MakeMeFaster;Trusted_Connection=True;Integrated Security=true;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
    }
}