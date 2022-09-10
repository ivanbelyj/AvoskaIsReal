using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Entities;
using AvoskaIsReal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Controllers
{
    public class ArticlesController : Controller
    {
        private DataManager _dataManager;
        private UserManager<User> _userManager;
        public ArticlesController(DataManager dataManager,
            UserManager<User> userManager)
        {
            _dataManager = dataManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            if (id == default(Guid))
            {
                return View(_dataManager.Articles.GetArticles()
                    .Where(article => article.CategoryName == Article.CATEGORY_THEORIES)
                    .ToList());
            }
            // return id == "id" ? View("Show") : NotFound();
            Article? article = _dataManager.Articles.GetArticleById(id);
            if (article is not null)
            {
                User? author = await _userManager.FindByIdAsync(article.UserId);

                ShowArticleViewModel model = new ShowArticleViewModel()
                {
                    Id = id,
                    Date = article.DateAdded,
                    Title = article.Title,
                    SubTitle = article.SubTitle,
                    Text = article.Text,
                    TitleImageUrl = article.TitleImageUrl,
                    AuthorsName = author?.UserName,
                    AuthorsAvatarUrl = author?.AvatarUrl,
                    AuthorsId = author?.Id
                };

                // Метатеги
                ViewBag.MetaTitle = article.Title;
                ViewBag.MetaDescription = article.MetaDescription;
                ViewBag.MetaKeywords = article.MetaKeywords;
                return View("Show", model);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult AllAboutAvoska()
        {
            // Выбрать статьи из категории "все об авоська" и передать
            ViewBag.textField = _dataManager.TextFields
                .GetTextFieldByCodeWord("AllAboutAvoska");
            return View(_dataManager.Articles.GetArticles()
                .Where(article => article.CategoryName == Article.CATEGORY_ABOUT_AVOSKA)
                .ToList());
        }
    }
}
