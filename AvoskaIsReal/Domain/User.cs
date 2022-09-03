using AvoskaIsReal.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AvoskaIsReal.Domain
{
    public class User : IdentityUser
    {
        public string? Career { get; set; }
        public string? About { get; set; }
        public string? Contacts { get; set; }
        public string? AvatarUrl { get; set; }

        // public List<Article> Articles { get; set; }
    }
}
