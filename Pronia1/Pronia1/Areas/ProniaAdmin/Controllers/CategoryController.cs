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
                .Where(c=>!c.IsDeleted)
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
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null) return NotFound();

            return View(category);
           
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category, int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Category? existed = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result=await  context.Categories.AnyAsync(c=>c.Name==category.Name &&  c.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists");
                return View();
            }
            existed.Name = category.Name;


            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Category? existed = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            existed.IsDeleted = !existed.IsDeleted;
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
