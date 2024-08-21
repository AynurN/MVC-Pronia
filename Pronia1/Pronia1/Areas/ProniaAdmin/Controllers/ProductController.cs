using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.Areas.ProniaAdmin.ViewModels;
using Pronia1.DAL;

namespace Pronia1.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext context;

        public ProductController(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetAdminProductVM> products = await context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Category)
                .Include(i => i.ProductImages.Where(x => x.isPrimary == true))
                .Select(p=> new GetAdminProductVM
                {
                    Name=p.Name,
                    Id=p.Id,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    ProductImage=p.ProductImages.FirstOrDefault().ImageURL
                })
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            CreateAdminProductVM productVM = new CreateAdminProductVM() {
             Categories= await context.Categories.Where(c => !c.IsDeleted).ToListAsync()
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAdminProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
                return View(productVM);
             
            }
            bool result = await context.Categories.AnyAsync(c => c.Id == productVM.CategoryId && !c.IsDeleted);
            if (!result) {
                ModelState.AddModelError("CategoryId", "Category does not exist!");
                productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
                return View(productVM);
                    }


                return RedirectToAction(nameof(Index));

        }
    }
}
