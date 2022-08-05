using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataManager _dataManager;

        public HomeController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public IActionResult Index()
        {
            return View(_dataManager.TextFields.GetTextFieldByCodeWord("Index"));
        }

        public IActionResult Contact()
        {
            return View(_dataManager.TextFields.GetTextFieldByCodeWord("Contact"));
        }
    }
}
