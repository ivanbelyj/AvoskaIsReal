using AvoskaIsReal.Domain;
using AvoskaIsReal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AvoskaIsReal.Domain.Entities;

namespace AvoskaIsReal.Components
{
    public class ArticleCards : ViewComponent
    {
        private DataManager _dataManager;
        private UserManager<User> _userManager;
        public ArticleCards(DataManager dataManager, UserManager<User> userManager)
        {
            _dataManager = dataManager;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(Article article)
        {
            // Article? article = _dataManager.Articles.GetArticleById(articleId);
            if (article is null)
                throw new ArgumentNullException(nameof(article));

            User author = await _userManager.FindByIdAsync(article.UserId);

            ShowArticleViewModel model = new ShowArticleViewModel()
            {
                Id = article.Id,
                Title = article.Title,
                SubTitle = article.SubTitle,
                Date = article.DateAdded,
                Text = article.Text,
                TitleImageUrl = article.TitleImageUrl,
                AuthorsName = author.UserName,
                AuthorsAvatarUrl = author.AvatarUrl
            };
            return View(model);
        }
    }
}
