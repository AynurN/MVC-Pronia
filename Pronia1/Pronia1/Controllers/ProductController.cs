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
        public async Task< IActionResult> Detail(int? id)
        {
            if(id==null || id <= 0)
            {
                return BadRequest();
            }
            Product? product = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.OrderByDescending(pi=>pi.isPrimary))
                .Include(p=>p.ProductTags)
                .ThenInclude(p=>p.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product == null)
            {
                return NotFound();
            }
            DetailVM detailVM = new DetailVM { Product=product, Products= await _context.Products.Where(p=>p.CategoryId==product.CategoryId && p.Id!=id).
                Include(p=>p.ProductImages.Where(x=>x.isPrimary!=null)).
                ToListAsync() };

            return View(detailVM);
        }
    }
}
