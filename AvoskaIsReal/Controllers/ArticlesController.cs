using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Controllers
{
    public class ArticlesController : Controller
    {
        private DataManager _dataManager;
        public ArticlesController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public IActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Todo: Выбрать статьи из категории "теории и доказательства"
                return View(_dataManager.Articles.GetArticles()
                    .Where(article => article.CategoryName == Article.CATEGORY_THEORIES)
                    .ToList());
            }
            return id == "id" ? View("Show") : NotFound();
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
