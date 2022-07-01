using System.ComponentModel.DataAnnotations;

namespace AvoskaIsReal.Domain.Entities
{
    public class TextField : PageEntityBase
    {
        [Required]
        public string CodeWord { get; set; }
    }
}
