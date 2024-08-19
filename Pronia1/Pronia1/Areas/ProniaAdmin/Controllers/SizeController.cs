using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;

namespace Pronia1.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext context;

        public SizeController(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> colors = await context.Sizes.ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await context.Sizes.AnyAsync(x => x.Name.Trim() == size.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View();
            }
            size.CreatedAt = DateTime.Now;
            await context.Sizes.AddAsync(size);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
