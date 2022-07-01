using System.ComponentModel.DataAnnotations;
namespace AvoskaIsReal.Domain.Entities
{
    public class Article : PageEntityBase
    {
        [Required(ErrorMessage = "Заполните название статьи")]
        [Display(Name = "Название статьи")]
        public string Title { get; set; }

        [Display(Name = "Краткое описание статьи")]
        public string? SubTitle { get; set; }

        [Display(Name = "Полный текст статьи")]
        public string? Text { get; set; }

    }
}
