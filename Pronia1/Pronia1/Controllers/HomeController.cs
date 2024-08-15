using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;
using Pronia1.ViewModels;

namespace Pronia1.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _context;
        public HomeController( AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            /*List<Slide> slides = new List<Slide>
            {
                new Slide
                {
                    
                    Title="Title1",
                    SubTitle="Subtitle1",
                    Description="Description1",
                    Order=1,
                    Image="1-1-524x617.png",
                    IsDeleted=false,
                    CreatedAt=DateTime.Now

                },
                new Slide
                {
                 
                    Title="Title2",
                    SubTitle="Subtitle2",
                    Description="Description2",
                    Order=2,
                    Image="1-2-524x617.png",
                    IsDeleted=false,
                    CreatedAt=DateTime.Now

                }
            };*/
           // _context.Slides.AddRange(slides);
            //_context.SaveChanges();
            HomeVM homeVM = new HomeVM
            {
                Slides = _context.Slides.OrderBy(x=>x.Order).Take(2).ToList(),
                Products = _context.Products
                .Include(x => x.ProductImages.Where(i => i.isPrimary != null))
                .OrderByDescending(p=>p.CreatedAt)
                .Take(8)
                .ToList()
        };

            return View(homeVM);
        }
    }
}
