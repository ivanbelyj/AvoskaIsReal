using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Components
{
    public class ArticleCards : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(5);
        }
    }
}
