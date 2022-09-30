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
            Claim? idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim is null)
                return Unauthorized();

            string usersId = idClaim.Value;
            ViewBag.returnUrl = "~/moderator/Articles/Index";
            return View("ArticlesList", _dataManager.Articles.GetArticles().ToList()
                .Where(article => article.UserId == usersId));
        }

        [HttpGet]
        public IActionResult Edit(Guid id, string returnUrl)
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

            ViewBag.returnUrl = returnUrl;
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Article article, string returnUrl)
        {
            if (article is null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                if (await IsAllowedToEditOrDeleteAsync(article))
                {
                    _dataManager.Articles.SaveArticle(article);

                    // Todo: image service, сохранение изображения статьи

                    if (returnUrl is not null)
                        return Redirect(returnUrl);
                    else
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

            // Если автор статьи уже удален, то он рассматривается как пользователь
            // без любых прав
            if (author is null)
                author = new User();

            // Если пользователь имеет право изменять другого пользователя,
            // то также имеет право изменять его статьи.
            return (await _authorizationService.AuthorizeAsync(User, author,
                   "EditOrDeleteUserPolicy")).Succeeded;
        }

        public async Task<IActionResult> Delete(Guid id, string? returnUrl)
        {
            if (id == default(Guid))
                return BadRequest();

            Article? article = _dataManager.Articles.GetArticleById(id);
            if (article is null)
                return NotFound();

            if (await IsAllowedToEditOrDeleteAsync(article))
            {
                _dataManager.Articles.DeleteArticle(id);

                if (returnUrl is not null)
                    return Redirect(returnUrl);
                else
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
