using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;

namespace Pronia1.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class CategoryController : Controller
    {
      
        private readonly AppDbContext context;

        public CategoryController(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await context.Categories
                .Include(x=>x.Products)
                .ToListAsync();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid){
                return View();
            }
            bool result = await context.Categories.AnyAsync(x => x.Name.Trim() == category.Name.Trim());
            if (result) {
                ModelState.AddModelError("Name", "Name already exists");
                return View();
            }
            category.CreatedAt=DateTime.Now;
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
        }
    }
}
