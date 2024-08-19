using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;

namespace Pronia1.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext context;

        public ColorController(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await context.Colors.ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await context.Colors.AnyAsync(x => x.Name.Trim() == color.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View();
            }
            color.CreatedAt = DateTime.Now;
            await context.Colors.AddAsync(color);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
