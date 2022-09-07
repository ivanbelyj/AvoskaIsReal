using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Entities;
using AvoskaIsReal.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AvoskaIsReal.Areas.Moderator.Controllers
{
    [Area("moderator")]
    public class ArticlesController : Controller
    {
        private DataManager _dataManager;
        public ArticlesController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public IActionResult Index()
        {
            // Todo: выдавать только те статьи, которые создал пользователь
            return View(_dataManager.Articles.GetArticles().ToList());
        }

        public IActionResult Edit(Guid id)
        {
            Article? article = null;
            if (id != default(Guid))
            {
                article = _dataManager.Articles.GetArticleById(id);
                if (article is null)
                    return NotFound();
            }

            if (article is null)
            {
                Claim? claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim is null)
                    return Unauthorized();

                article = new Article() { UserId = claim.Value, Text="" };
            }
            return View(article);
        }

        [HttpPost]
        public IActionResult Edit(Article article)
        {
            if (article is null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                // Todo: Проверить, может ли пользователь изменять/создавать статью
                _dataManager.Articles.SaveArticle(article);
                return RedirectToAction(nameof(ArticlesController.Index),
                    nameof(ArticlesController).CutController());
            }

            return View(article);
        }
    }
}
