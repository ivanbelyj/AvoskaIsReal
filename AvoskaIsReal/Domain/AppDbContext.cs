using Microsoft.EntityFrameworkCore;
using AvoskaIsReal.Domain.Entities;

namespace AvoskaIsReal.Domain
{
    public class AppDbContext : DbContext
    {
        private DbSet<Article> Articles { get; set; }
        private DbSet<Article> TextFields { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }
    }
}
