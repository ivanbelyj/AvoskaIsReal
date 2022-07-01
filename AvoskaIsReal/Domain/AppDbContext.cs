using Microsoft.EntityFrameworkCore;
using AvoskaIsReal.Domain.Entities;

namespace AvoskaIsReal.Domain
{
    public class AppDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<TextField> TextFields { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }
    }
}
