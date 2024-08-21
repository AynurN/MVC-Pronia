using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia1.Areas.ProniaAdmin.ViewModels;
using Pronia1.DAL;
using Pronia1.Models;
using Pronia1.Utilities.Enums;
using Pronia1.Utilities.Extensions;
using System.Reflection;

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
        public async  Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            //if(!ModelState.IsValid)
            //return View(slide);
            if (!slideVM.Photo.ValidateType("image/")) {
                ModelState.AddModelError("Photo", "Type is not valid!");
                return View();
            }
            if (!slideVM.Photo.ValidateSize(FileSize.MB,2))
            {
                ModelState.AddModelError("Photo", "File size is more than 2Mb!");
                return View();
            }

            string fileName= await slideVM.Photo.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images");

            Slide slide = new Slide()
            {
                Title = slideVM.Title,
                SubTitle = slideVM.SubTitle,
                Description = slideVM.Description,
                Order = slideVM.Order,
                CreatedAt = DateTime.Now,
                Image = fileName
            };
            await context.Slides.AddAsync(slide);
           await  context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            Slide? slide = await context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();
            slide.Image.DeleteFile(env.WebRootPath, "assets", "images", "website-images");
            context.Slides.Remove(slide);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));

    }
      
        public async Task<IActionResult> Update(int? id)
        {

            if (id is null) return BadRequest();
            Slide? slide = await context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();
            UpdateSlideVM slideVM = new UpdateSlideVM() {
                Title=slide.Title,
                SubTitle=slide.SubTitle,
                Description = slide.Description,
                Order = slide.Order,
                Image = slide.Image
            };
            return View(slideVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id,UpdateSlideVM slideVM)
        {
            if (!ModelState.IsValid) return View(slideVM);
            Slide? exist = await context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (exist is null) return NotFound();
            if (slideVM.Photo is not null)
            {
                if (!slideVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "FileType is not valid!");
                    return View(slideVM);
                }
                if (!slideVM.Photo.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File Size is not valid!");
                    return View(slideVM);

                }

            }
            string fileName = await slideVM.Photo.CreateFileAsync(env.WebRootPath, "assets", "images", "website-images");
            exist.Image.DeleteFile(env.WebRootPath, "assets", "images", "website-images");
            exist.Image = fileName;
            exist.Title = slideVM.Title;
            exist.SubTitle = slideVM.SubTitle;
            exist.Description = slideVM.Description;
            exist.Order = slideVM.Order;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

    }
   

}
