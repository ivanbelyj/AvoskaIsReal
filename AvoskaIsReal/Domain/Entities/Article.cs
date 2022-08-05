using System.ComponentModel.DataAnnotations;
namespace AvoskaIsReal.Domain.Entities
{
    public class Article : PageEntityBase
    {
        [Required(ErrorMessage = "Заполните название статьи")]
        [Display(Name = "Название статьи")]
        public override string Title { get; set; }

        [Display(Name = "Содержание статьи")]
        public override string Text { get; set; }

        [Display(Name = "Краткое описание статьи")]
        public string? SubTitle { get; set; }
    }
}
