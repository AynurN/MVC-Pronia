using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;
using Pronia1.ViewModels;

namespace Pronia1.Controllers
{
    public class ProductController : Controller
    {
        AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if(id==null || id <= 0)
            {
                return BadRequest();
            }
            Product? product = _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.OrderByDescending(pi=>pi.isPrimary))
                .FirstOrDefault(p => p.Id == id);
                
            if (product == null)
            {
                return NotFound();
            }
            DetailVM detailVM = new DetailVM { Product=product, Products=_context.Products.Where(p=>p.CategoryId==product.CategoryId && p.Id==id).
                Include(p=>p.ProductImages.Where(x=>x.isPrimary!=null)).
                ToList() };

            return View(detailVM);
        }
    }
}
