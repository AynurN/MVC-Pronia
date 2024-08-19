using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;

namespace Pronia1.Areas.ProniaAdmin.Controllers
{
    public class TagController : Controller
    {
        private readonly AppDbContext context;

        public TagController(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Tag> colors = await context.Tags.ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await context.Tags.AnyAsync(x => x.Name.Trim() == tag.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View();
            }
            tag.CreatedAt = DateTime.Now;
            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
