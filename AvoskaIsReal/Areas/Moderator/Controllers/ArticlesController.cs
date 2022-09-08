using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Entities;
using AvoskaIsReal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AvoskaIsReal.Areas.Moderator.Controllers
{
    [Area("moderator")]
    public class ArticlesController : Controller
    {
        private DataManager _dataManager;
        private IAuthorizationService _authorizationService;
        private UserManager<User> _userManager;
        public ArticlesController(DataManager dataManager,
            IAuthorizationService authorizationService, UserManager<User> userManager)
        {
            _dataManager = dataManager;
            _authorizationService = authorizationService;
            _userManager = userManager;
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
                ViewBag.creatingNewArticle = true;
                article = new Article() { UserId = claim.Value, Text = "" };
            }
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Article article)
        {
            if (article is null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                // Todo: политику авторизации можно переименовать
                if (await IsAllowedToEditOrDeleteAsync(article))
                {
                    _dataManager.Articles.SaveArticle(article);
                    return RedirectToAction(nameof(ArticlesController.Index),
                        nameof(ArticlesController).CutController());
                }
                else
                    return Unauthorized();
            }

            return View(article);
        }


        private async Task<bool> IsAllowedToEditOrDeleteAsync(Article article)
        {
            User author = await _userManager.FindByIdAsync(article.UserId);
            // Todo: что, если user - null, т.е. автора статьи уже нет?

            // Если пользователь имеет право изменять другого пользователя,
            // то также имеет право изменять его статьи.
            return (await _authorizationService.AuthorizeAsync(User, author,
                   "EditOrDeleteUserPolicy")).Succeeded;
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == default(Guid))
                return BadRequest();

            Article? article = _dataManager.Articles.GetArticleById(id);
            if (article is null)
                return NotFound();

            if (await IsAllowedToEditOrDeleteAsync(article))
            {
                _dataManager.Articles.DeleteArticle(id);
                return RedirectToAction(nameof(ArticlesController.Index),
                    nameof(ArticlesController).CutController());
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
