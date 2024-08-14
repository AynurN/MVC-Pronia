using Microsoft.AspNetCore.Mvc;
using Pronia1.Models;
using Pronia1.ViewModels;

namespace Pronia1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Slide> slides = new List<Slide>
            {
                new Slide
                {
                    Id=1,
                    Title="Title1",
                    SubTitle="Subtitle1",
                    Description="Description1",
                    Order=1,
                    Image="1-1-524x617.png"

                },
                new Slide
                {
                    Id=2,
                    Title="Title2",
                    SubTitle="Subtitle2",
                    Description="Description2",
                    Order=2,
                    Image="1-2-524x617.png"

                },new Slide
                {
                    Id=3,
                    Title="Title3",
                    SubTitle="Subtitle3",
                    Description="Description3",
                    Order=3

                }
            };
            HomeVM homeVM = new HomeVM
            {
                Slides = slides.OrderBy(x=>x.Order).Take(2).ToList()
        };

            return View(homeVM);
        }
    }
}
