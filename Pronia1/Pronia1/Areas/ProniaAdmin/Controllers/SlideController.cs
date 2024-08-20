using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.DAL;
using Pronia1.Models;
using Pronia1.Utilities.Enums;
using Pronia1.Utilities.Extensions;

namespace Pronia1.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }
        public async  Task<IActionResult> Index()
        {
            List<Slide> slides = await context.Slides.Where(s => !s.IsDeleted).ToListAsync();
            return View(slides);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async  Task<IActionResult> Create(Slide slide)
        {
            //if(!ModelState.IsValid)
            //return View(slide);
            if (!slide.Photo.ValidateType("image/")) {
                ModelState.AddModelError("Photo", "Type is not valid!");
                return View();
            }
            if (!slide.Photo.ValidateSize(FileSize.MB,2))
            {
                ModelState.AddModelError("Photo", "File size is more than 2Mb!");
                return View();
            }


            slide.Image = await slide.Photo.CreateFileAsync(env.WebRootPath,"assets", "images", "website-images");
              slide.CreatedAt = DateTime.Now;
            await context.Slides.AddAsync(slide);
           await  context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}
