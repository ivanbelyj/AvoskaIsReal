using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Admin.Controllers
{
    [Area("admin")]
    public class TextFieldsController : Controller
    {
        private DataManager _dataManager;

        public TextFieldsController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(string codeWord)
        {
            TextField? textField = _dataManager.TextFields.GetTextFieldByCodeWord(codeWord);
            if (textField == null)
                return NotFound();
            return View(textField);
        }

        [HttpPost]
        public IActionResult Edit(TextField textField)
        {
            if (ModelState.IsValid)
            {
                _dataManager.TextFields.SaveTextField(textField);
                return RedirectToAction("Index", "TextFields");
            }
            return View(textField);
        }
    }
}
