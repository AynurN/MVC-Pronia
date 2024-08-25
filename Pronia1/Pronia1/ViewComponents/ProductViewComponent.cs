using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;
using Pronia1.Utilities.Enums;

namespace Pronia1.ViewComponents
{
    public class ProductViewComponent :ViewComponent
    {
        private readonly AppDbContext context;

        public ProductViewComponent(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(SortType sort)
        {
            //var products = await context.Products.Include(p=>p.ProductImages).Where(p => !p.IsDeleted).ToListAsync();
            IQueryable<Product> productsQuery = context.Products.Where(p => !p.IsDeleted);
            switch (sort)
            {
                case SortType.Name:
                    productsQuery = productsQuery.OrderBy(p => p.Name);
                    break;
                case SortType.Price:
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case SortType.Latest:
                    productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt);
                    break;

            }
            productsQuery = productsQuery.Take(8).Include(p => p.ProductImages.Where(pi => pi.isPrimary != null));

            return View(await productsQuery.ToListAsync());
        }
    }
}
