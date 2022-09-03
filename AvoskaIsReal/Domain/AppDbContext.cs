using Microsoft.EntityFrameworkCore;
using AvoskaIsReal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AvoskaIsReal.Domain
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<TextField> TextFields { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<Article>()
        //        .HasOne(x => x.User)
        //        .WithMany(x => x.Articles)
        //        .HasForeignKey(x => x.UserId)
        //        .HasPrincipalKey(user => user.Id);
        //}
    }
}
