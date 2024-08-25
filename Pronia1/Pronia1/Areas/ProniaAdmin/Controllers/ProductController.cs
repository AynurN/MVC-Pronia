using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Pronia1.Areas.ProniaAdmin.ViewModels;
using Pronia1.DAL;
using Pronia1.Models;
using Pronia1.Utilities.Enums;
using Pronia1.Utilities.Extensions;

namespace Pronia1.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetAdminProductVM> products = await context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Category)
                .Include(i => i.ProductImages)
                .Select(p=> new GetAdminProductVM
                {
                    Name=p.Name,
                    Id=p.Id,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    ProductImage=p.ProductImages.FirstOrDefault(x => x.isPrimary == true).ImageURL
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
                productVM.Tags = await context.Tags.Where(t => !t.IsDeleted).ToListAsync();
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("MainPhoto", "Image type is not valid!");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(FileSize.MB, 1))
            {
                ModelState.AddModelError("MainPhoto", "Image size is not valid!");
                return View(productVM);
            }

            if (!productVM.HoverPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("HoverPhoto", "Image type is not valid!");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateSize(FileSize.MB, 1))
            {
                ModelState.AddModelError("HoverPhoto", "Image size is not valid!");
                return View(productVM);
            }
            bool categoryExists = await context.Categories.AnyAsync(c => c.Id == productVM.CategoryId && !c.IsDeleted);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryId", "Category does not exist!");
                productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
                productVM.Tags = await context.Tags.Where(t => !t.IsDeleted).ToListAsync();
                return View(productVM);
            }

            if (productVM.TagIds != null)
            {
                var validTagIds = await context.Tags.Where(t => !t.IsDeleted).Select(t => t.Id).ToListAsync();
                bool invalidTagSelected = productVM.TagIds.Any(tId => !validTagIds.Contains(tId));
                if (invalidTagSelected)
                {
                    ModelState.AddModelError("TagIds", "One or more selected tags do not exist!");
                    productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
                    productVM.Tags = await context.Tags.Where(t => !t.IsDeleted).ToListAsync();
                    return View(productVM);
                }
            }

            ProductImage mainImage = new ProductImage
            {
                IsDeleted = false,
                CreatedAt=DateTime.Now,
                isPrimary=true,
                ImageURL=await productVM.MainPhoto.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images")

            };
            ProductImage hoverImage = new ProductImage
            {
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                isPrimary = false,
                ImageURL = await productVM.HoverPhoto.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images")

            };

            Product product = new Product
            {
                Name = productVM.Name,
                CategoryId = productVM.CategoryId,
                SKU = productVM.SKU,
                Descroiption= productVM.Description,
                Price = productVM.Price,
                CreatedAt = DateTime.Now,
                ProductImages= new List<ProductImage>{ mainImage,hoverImage}
            };
            string errorText= string.Empty;
            if (productVM.Photos is not null)
            {
                foreach (IFormFile file in productVM.Photos)
                {
                    if (!file.ValidateType("image/"))
                    {
                        errorText += $"{file.Name} type is not valid!";
                        continue;
                    }
                    if (!file.ValidateSize(FileSize.MB, 1))
                    {
                        errorText += $"{file.Name} size is not valid!";
                        continue;
                    }
                    ProductImage image = new ProductImage
                    {
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        isPrimary = null,
                        ImageURL = await file.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images")

                    };
                    product.ProductImages.Add(image);
                }

                TempData["ErrorMessage"] = errorText;


            }

            if (productVM.TagIds != null)
            {
                product.ProductTags = productVM.TagIds.Select(tId => new ProductTag
                {
                    TagId = tId
                }).ToList();
            }

            await context.AddAsync(product);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Product? product = await context.Products.Include(p=>p.ProductTags).Include(p=>p.ProductImages).Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.Id == id);
            if( product is null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Descroiption,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync(),
                Tags= await context.Tags.Where(c => !c.IsDeleted).ToListAsync(),
                TagIds=product.ProductTags.Select(pt=>pt.TagId).ToList()

        };
            if(product.ProductImages is not null)
            {
                productVM.Images = product.ProductImages.ToList();
            }
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id is null || id < 1) return BadRequest();
            Product? exist = await context.Products.Include(p => p.ProductTags).Include(p=>p.ProductImages).Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.Id == id);
            if (exist is null) return NotFound();
            productVM.Categories = await context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            productVM.Tags = await context.Tags.Where(c => !c.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);

            }
            if(productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "Image type is not valid!");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(FileSize.MB, 1))
                {
                    ModelState.AddModelError("MainPhoto", "Image size is not valid!");
                    return View(productVM);
                }

            }
            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError("HoverPhoto", "Image type is not valid!");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.ValidateSize(FileSize.MB, 1))
                {
                    ModelState.AddModelError("HoverPhoto", "Image size is not valid!");
                    return View(productVM);
                }

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
            
            if(productVM.MainPhoto is not null)
            {
                ProductImage productImage = new ProductImage {
                    CreatedAt = DateTime.Now,
                    isPrimary = true,
                    ImageURL = await productVM.MainPhoto.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images"),
                    IsDeleted = false
                };
                ProductImage existMain = exist.ProductImages.FirstOrDefault(pi => pi.isPrimary == true);
                existMain.ImageURL.DeleteFile(env.WebRootPath, "assets", "images", "website-images");
                exist.ProductImages.Remove(existMain);
                exist.ProductImages.Add(productImage);

            }
            if (productVM.HoverPhoto is not null)
            {
                ProductImage productImage = new ProductImage
                {
                    CreatedAt = DateTime.Now,
                    isPrimary = false,
                    ImageURL = await productVM.HoverPhoto.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images"),
                    IsDeleted = false
                };
                ProductImage existHover = exist.ProductImages.FirstOrDefault(pi => pi.isPrimary == false);
                existHover.ImageURL.DeleteFile(env.WebRootPath, "assets", "images", "website-images");
                exist.ProductImages.Remove(existHover);
                exist.ProductImages.Add(productImage);

            }
            
            if (productVM.ImageIds is null)
            {
              productVM.ImageIds=new List<int>();
            }
            var deleteImages = exist.ProductImages?.Where(pi => !productVM.ImageIds.Exists(imgId => pi.Id == imgId) && pi.isPrimary==null).ToList();
            foreach (var dImg in deleteImages)
            {
                dImg.ImageURL.DeleteFile(env.WebRootPath, "assets", "images", "website-images");
            }
            if(productVM.Photos is not null)
            {
                string errorText = string.Empty;
                foreach (var file in productVM.Photos)
                {
                    if (!file.ValidateType("image/"))
                    {
                        errorText += $"{file.Name} type is not valid!";
                        continue;
                    }
                    if (!file.ValidateSize(FileSize.MB, 1))
                    {
                        errorText += $"{file.Name} size is not valid!";
                        continue;
                    }
                    ProductImage image = new ProductImage
                    {
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        isPrimary = null,
                        ImageURL = await file.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images")

                    };
                    exist.ProductImages.Add(image);

                }
                TempData["ErrorMessage"] = errorText;

            }

           


            context.ProductImages.RemoveRange(deleteImages);
            if (productVM.TagIds is not null)
            {
                var deletedTags = exist.ProductTags.Where(pt => !productVM.TagIds.Exists(tid => tid == pt.Id)).ToList();
                context.ProductTags.RemoveRange(deletedTags);
                exist.ProductTags.AddRange(productVM.TagIds.Where(tid => !exist.ProductTags.Any(pt => pt.TagId == tid)).Select(tid => new ProductTag { TagId = tid }));
            }
            else
            {
                foreach (var item in exist.ProductTags)
                {
                    exist.ProductTags.Remove(item);
                    
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
