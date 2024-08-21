using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.Areas.ProniaAdmin.ViewModels;
using Pronia1.DAL;
using Pronia1.Models;

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
             Categories= await context.Categories.Where(c => !c.IsDeleted).ToListAsync(),
                Tags = await context.Tags.Where(c => !c.IsDeleted).ToListAsync(),
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAdminProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
                productVM.Tags = await context.Tags.Where(c => !c.IsDeleted).ToListAsync();
                return View(productVM);
             
            }
            bool result = await context.Categories.AnyAsync(c => c.Id == productVM.CategoryId && !c.IsDeleted);
            if (!result) {
                ModelState.AddModelError("CategoryId", "Category does not exist!");
                productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
                productVM.Tags = await context.Tags.Where(c => !c.IsDeleted).ToListAsync();
                return View(productVM);
                    }
           bool tagResult= productVM.TagIds.Any(tId =>!productVM.Tags.Exists(t => t.Id == tId));
            if (tagResult)
            {
                ModelState.AddModelError("TagIds", "Tag does not exist!");
                return View(productVM);
            }
            Product product = new Product()
            {
                Name = productVM.Name,
                CategoryId = productVM.CategoryId,
                SKU = productVM.SKU,
                Descroiption = productVM.Description,
                Price = productVM.Price,
                ProductTags = productVM.TagIds.Select(tId => new ProductTag
                {
                    TagId = tId
                }).ToList(),
                CreatedAt = DateTime.Now
            };
            await context.AddAsync(product);
            await context.SaveChangesAsync(); 

                return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Product? product = await context.Products.Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.Id == id);
            if( product is null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Descroiption,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync()
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id is null || id < 1) return BadRequest();
            Product? exist = await context.Products.Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.Id == id);
            if (exist is null) return NotFound();
            productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);

            }
            if (exist.CategoryId != productVM.CategoryId)
            {
                bool result= await context.Categories.AnyAsync(c => c.Id == productVM.CategoryId && !c.IsDeleted);
                if (!result)
                {
                    ModelState.AddModelError("Category", "Category does not exist!");
                    return View(productVM);
                }
            }
            exist.Name = productVM.Name;
            exist.SKU = productVM.SKU;
            exist.Price = productVM.Price;
            exist.CategoryId = productVM.CategoryId.Value;
            exist.Descroiption = productVM.Description;
            await context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
    }
}
