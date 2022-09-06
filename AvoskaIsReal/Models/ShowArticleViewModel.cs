namespace AvoskaIsReal.Models
{
    public class ShowArticleViewModel
    {
        // Todo: нужно ли добавить атрибуты вроде Required?
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? TitleImageUrl { get; set; }
        public string? SubTitle { get; set; }
        public string? AuthorsAvatarUrl { get; set; }
        public string AuthorsName { get; set; }

        // В identity id - string
        public string AuthorsId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
