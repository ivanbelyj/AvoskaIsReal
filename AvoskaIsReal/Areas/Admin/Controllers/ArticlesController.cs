using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ArticlesController : Controller
    {
        private readonly DataManager _dataManager;
        public ArticlesController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        public IActionResult AllArticles()
        {
            ViewBag.returnUrl = "/admin/Articles/AllArticles";
            return View("ArticlesList", _dataManager.Articles.GetArticles().ToList());
        }
    }
}
