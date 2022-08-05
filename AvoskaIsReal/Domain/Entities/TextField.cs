using System.ComponentModel.DataAnnotations;

namespace AvoskaIsReal.Domain.Entities
{
    public class TextField : PageEntityBase
    {
        [Required]
        public string CodeWord { get; set; }

        [Display(Name = "Название страницы (заголовок)")]
        public override string Title { get; set; } = "Информационная страница";

        [Display(Name = "Содержание страницы")]
        public override string Text { get; set; } = "Содержание заполняется модераторами";
    }
}
