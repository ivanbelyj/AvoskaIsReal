using AvoskaIsReal.Domain;
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
                return View();
            }
            return id == "id" ? View("Show") : NotFound();
        }

        public IActionResult AllAboutAvoska()
        {
            return View(_dataManager.TextFields.GetTextFieldByCodeWord("AllAboutAvoska"));
        }
    }
}
