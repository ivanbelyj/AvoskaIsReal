using System.ComponentModel.DataAnnotations;

namespace AvoskaIsReal.Domain.Entities
{
    public abstract class PageEntityBase
    {
        public PageEntityBase() => DateAdded = DateTime.UtcNow;

        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Название (заголовок)")]
        public virtual string Title { get; set; }

        [Display(Name = "Содержание страницы")]
        public virtual string Text { get; set; }

        /*[Display(Name = "SEO метатег title")]
        public string? MetaTitle { get; set; }*/

        [Display(Name = "SEO метатег keywords")]
        public string? MetaKeywords { get; set; }

        [Display(Name = "SEO метатег description")]
        public string? MetaDescription { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateAdded { get; set; }
    }
}
