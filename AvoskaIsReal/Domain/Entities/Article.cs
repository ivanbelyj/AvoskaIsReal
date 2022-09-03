using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Display(Name = "Url заголовочного изображения статьи")]
        public string? TitleImageUrl { get; set; }

        public string UserId { get; set; }

        // Навигационное свойство
        // [ForeignKey("UserId")]
        // public User User { get; set; }

        // Категории две: theories и about-avoska
        public string CategoryName { get; set; } = CATEGORY_THEORIES;

        public const string CATEGORY_THEORIES = "theories";
        public const string CATEGORY_ABOUT_AVOSKA = "about-avoska";
    }
}
