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
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Slides = await _context.Slides.OrderBy(x=>x.Order).Take(2).ToListAsync(),
                Products = await _context.Products
                .Include(x => x.ProductImages.Where(i => i.isPrimary != null))
                .OrderByDescending(p=>p.CreatedAt)
                .Take(8)
                .ToListAsync()
        };

            return  View(homeVM);
        }
    }
}
